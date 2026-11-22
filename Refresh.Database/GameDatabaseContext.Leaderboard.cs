using JetBrains.Annotations;
using MongoDB.Bson;
using Refresh.Database.Query;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Activity;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Database.Models.Users;
using Refresh.Database.Models.Levels;
using Refresh.Common.Helpers;

namespace Refresh.Database;

public partial class GameDatabaseContext // Leaderboard
{
    private IQueryable<GameScore> GameScoresIncluded => this.GameScores
        .Include(s => s.Publisher)
        .Include(s => s.Level)
        .Include(s => s.Level.Publisher)
        .Include(s => s.Participants.OrderBy(s => s.PlayerNumber));

    private IQueryable<GameScoreParticipant> GameScoreParticipantsIncluded => this.GameScoreParticipants
        .Include(s => s.User)
        .Include(s => s.User!.Statistics)
        .Include(s => s.Score)
        .Include(s => s.Score!.Publisher)
        .Include(s => s.Score!.Publisher!.Statistics)
        .Include(s => s.Score!.Level)
        .Include(s => s.Score!.Level.Statistics)
        .Include(s => s.Score!.Level.Publisher)
        .Include(s => s.Score!.Level.Publisher!.Statistics);
    
    public GameScore SubmitScore(ISerializedScore score, Token token, GameLevel level)
        => this.SubmitScore(score, token.User, level, token.TokenGame, token.TokenPlatform);

    public GameScore SubmitScore(ISerializedScore score, GameUser user, GameLevel level, TokenGame game, TokenPlatform platform)
    {
        // Throw incase the method directly gets called like this in a test
        if (score.PlayerUsernames.Count <= 0)
        {
            throw new ArgumentException("Player list is empty!", nameof(score.PlayerUsernames));
        }
        if (!score.PlayerUsernames.Contains(user.Username))
        {
            throw new ArgumentException("Player list doesn't contain uploader's username!", nameof(score.PlayerUsernames));
        }

        // Get all current #1 holders, so we can know later who to send overtake notifications to.
        // Only need the publishers, and not all participants, because all clients in a lobby will upload their score
        // themselves when completing a level, not just the host, so they will all be publishers.
        List<GameUser> currentNumberOnes = this.GameScores
            .Include(s => s.Publisher)
            // Ensure we only get scores from this exact leaderboard (they are identified by level ID + score type),
            // and only get the current #1s.
            .Where(s => s.LevelId == level.LevelId && s.ScoreType == score.ScoreType && s.Rank == 1)
            // This makes sure that we don't send notifications if the current #1s have a score of 0 (or below if PSP),
            // and that we only send the notif if our new score is actually greater than that of the current #1s
            // (otherwise we'd end up with no scores, and therefore no users at all here).
            .Where(s => s.Score > 0 && s.Score < score.Score)
            // Don't notify any of these users if they've also participated in this score (which includes the publisher themselves)
            .Where(s => !score.PlayerUsernames.Contains(s.Publisher.Username))
            .Select(s => s.Publisher)
            .ToList();

        GameScore newScore = new()
        {
            Score = score.Score,
            ScoreType = score.ScoreType,
            Level = level,
            Publisher = user,
            ScoreSubmitted = this._time.Now,
            Game = game,
            Platform = platform,
        };

        this.WriteEnsuringStatistics(level, () =>
        {
            this.GameScores.Add(newScore);
            level.Statistics!.CompletionCount++;
        });

        List<GameScoreParticipant> participants = [];
        int playerNumber = 1;

        // No need to get publisher's user, since we already have it.
        // Insert separately, and then remove name from the list so we don't iterate over it below.
        this.AddScoreParticipant(newScore, playerNumber, user.Username, user, false);
        score.PlayerUsernames.Remove(user.Username);
        playerNumber++;

        // Unlike photo/report subjects, these are just names. They don't have any additional data uploaded by the game
        // (e.g. boundaries), so we can safely only save distinct names without losing potentially useful data.
        foreach (string username in score.PlayerUsernames.Distinct())
        {
            // From now on, we will also save usernames of users who we don't have a GameUser of (likely from a different server).
            this.AddScoreParticipant(newScore, playerNumber, username, this.GetUserByUsername(username, true), false);
            playerNumber++;
        }
        this.SaveChanges();

        this.RecalculateScoreStatistics(level.LevelId, score.ScoreType); // TODO move rank into own DB table, and have a job do this, like with other stats
        this.CreateLevelScoreEvent(user, newScore);

        // Now we can take care of sending the notif. Could probably also do it right after the initial insertion, but whatever.
        // NOTE: If you want to change the notif text, make sure to adjust the respective Assert in 
        // ScoreLeaderboardTests.OnlySendOvertakeNotifsToRelevantPlayers() aswell!
        foreach (GameUser overtakenUser in currentNumberOnes)
        {
            this.AddNotification("Score overtaken", 
                $"Your #1 score on {level.Title} has been overtaken by {StringHelper.FormatListingOfStrings(score.PlayerUsernames)} in {score.ScoreType}-player-mode!", 
                overtakenUser, "medal");
        }

        return newScore;
    }
    
    public GameScoreParticipant AddScoreParticipant(GameScore score, int playerNumber, string username, GameUser? user, bool save = true)
    {
        GameScoreParticipant participant = new()
        {
            ScoreId = score.ScoreId,
            PlayerNumber = playerNumber,
            Username = username,
            User = user,
        };

        this.GameScoreParticipants.Add(participant);
        if (save) this.SaveChanges();
        return participant;
    }

    /// <param name="scoreType">0 = don't filter by type</param>
    public DatabaseScoreList GetTopScoresForLevel(GameLevel level, int count, int skip, byte scoreType, bool showDuplicates = false, DateTimeOffset? minAge = null, GameUser? user = null)
    {
        IEnumerable<GameScore> scores = this.GameScoresIncluded
            .Where(s => s.LevelId == level.LevelId)
            .OrderByDescending(s => s.Score);
        
        if (scoreType != 0)
            scores = scores.Where(s => s.ScoreType == scoreType);

        if (!showDuplicates)
            scores = scores.Where(s => s.Rank != 0);
        
        if (minAge != null)
            scores = scores.Where(s => s.ScoreSubmitted >= minAge);

        return new(scores.ToArray().Select(s => new ScoreWithRank(s, s.Rank)), skip, count, user);
    }
    
    // TODO all these score-specific extra params should probably be passed in by a dedicated struct, like with level params.
    public DatabaseList<ScoreWithRank> GetTopScoresByUser(GameUser user, int count, int skip, byte scoreType, uint minRank, uint maxRank, bool showDuplicates = false)
    {
        IEnumerable<GameScore> scores = this.GameScoresIncluded
            .Where(s => s.PublisherId == user.UserId)
            .OrderByDescending(s => s.Score);
        
        if (scoreType != 0)
            scores = scores.Where(s => s.ScoreType == scoreType);

        if (!showDuplicates)
            scores = scores.Where(s => s.Rank != 0);
        
        if (minRank > 0)
            scores = scores.Where(s => s.Rank >= minRank);
        
        if (maxRank > 0)
            scores = scores.Where(s => s.Rank <= maxRank);

        return new(scores.ToArray().Select(s => new ScoreWithRank(s, s.Rank)), skip, count);
    }

    public DatabaseScoreList GetRankedScoresAroundScore(GameScore score, int count, GameUser? user = null)
    {
        if (count % 2 != 1) throw new ArgumentException("The number of scores must be odd.", nameof(count));
        
        // this is probably REALLY fucking slow, and i probably shouldn't be trusted with LINQ anymore

        List<GameScore> scores = this.GameScoresIncluded
            .Where(s => s.ScoreType == score.ScoreType && s.LevelId == score.LevelId)
            .OrderByDescending(s => s.Score)
            .Where(s => s.Rank != 0)
            .ToList();

        return new
        (
            scores.Select(s => new ScoreWithRank(s, s.Rank)),
            Math.Min(scores.Count, scores.IndexOf(score) - count / 2), // center user's score around other scores
            count, user
        );
    }
    
    /// <param name="scoreType">0 = don't filter by type</param>
    public DatabaseScoreList GetLevelTopScoresByFriends(GameUser user, GameLevel level, int skip, int count, byte scoreType, DateTimeOffset? minAge = null)
    {
        IEnumerable<ObjectId> mutuals = this.GetUsersMutuals(user)
            .Select(u => u.UserId)
            .Append(user.UserId);

        IEnumerable<GameScore> scores = this.GameScoresIncluded
            .Where(s => s.LevelId == level.LevelId)
            .OrderByDescending(s => s.Score)
            .Where(s => s.Rank != 0)
            .Where(s => mutuals.Contains(s.PublisherId));
        
        if (scoreType != 0)
            scores = scores.Where(s => s.ScoreType == scoreType);
        
        if (minAge != null)
            scores = scores.Where(s => s.ScoreSubmitted >= minAge);

        return new(scores.ToArray().Select(s => new ScoreWithRank(s, s.Rank)), skip, count, user);
    }

    [Pure]
    [ContractAnnotation("null => null; notnull => canbenull")]
    public GameScore? GetScoreByUuid(string? uuid)
    {
        if (uuid == null) return null;
        if(!ObjectId.TryParse(uuid, out ObjectId objectId)) return null;

        return GetScoreByObjectId(objectId);
    }
    
    [Pure]
    [ContractAnnotation("null => null; notnull => canbenull")]
    public GameScore? GetScoreByObjectId(ObjectId? id)
    {
        if (id == null) return null;
        return this.GameScoresIncluded
            .FirstOrDefault(u => u.ScoreId == id);
    }
    
    public void DeleteScore(GameScore score)
    {
        IQueryable<Event> scoreEvents = this.Events
            .Where(e => e.StoredDataType == EventDataType.Score && e.StoredObjectId == score.ScoreId);
        
        this.Events.RemoveRange(scoreEvents);
        this.GameScores.Remove(score);
        this.SaveChanges();

        // Only recalculate ranks if the score was a high-score.
        // Also, separate write transaction because otherwise recalculation would still include the unwanted scores.
        if (score.Rank != 0)
            this.RecalculateScoreStatistics(score.LevelId, score.ScoreType, true);
    }
    
    public void DeleteScoresSetByUser(GameUser user)
    {
        // Find out which leaderboards we will have to recalculate the ranks of.
        // Only need high-scores for this, as the other, overtaken scores should not affect ranking at all,
        // and additionally, this way we will always have not more than 1 score per level/score type,
        // avoiding recalculation of the same leaderboards multiple times.
        IEnumerable<GameScore> scores = this.GameScores
            .Where(s => s.PublisherId == user.UserId && s.Rank != 0)
            .ToArray();

        this.GameScores.RemoveRange(s => s.PublisherId == user.UserId);
        this.Events.RemoveRange(s => s.StoredDataType == EventDataType.Score && s.UserId == user.UserId);
        this.SaveChanges();

        foreach (GameScore score in scores)
        {
            this.RecalculateScoreStatistics(score.LevelId, score.ScoreType, false);
        }
        this.SaveChanges();
    }

    public IEnumerable<GameUser> GetPlayersFromScore(GameScore score)
    {
        return this.GameScoreParticipantsIncluded
            .Where(s => s.ScoreId == score.ScoreId && s.User != null)
            .Select(s => s.User!);
    }

    public int GetTotalPlayersFromScoreCount(GameScore score)
    {
        return this.GameScoreParticipants
            .Count(s => s.ScoreId == score.ScoreId);
    }
    
    public GameUser? GetSubmittingPlayerFromScore(GameScore score)
    {
        return score.Publisher;
    }
}