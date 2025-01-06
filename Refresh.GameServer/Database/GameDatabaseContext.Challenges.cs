using Refresh.GameServer.Types.Challenges.LbpHub;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Database;

public partial class GameDatabaseContext // Challenges
{
    #region Challenge

    public GameChallenge CreateChallenge(SerializedChallenge createInfo, GameLevel level, GameUser user)
    {
        DateTimeOffset now = this._time.Now;

        GameChallenge challenge = new()
        {
            Name = createInfo.Name,
            Publisher = user,
            Level = level,
            StartCheckpointUid = createInfo.StartCheckpointUid,
            EndCheckpointUid = createInfo.EndCheckpointUid,
            // Take the type of the first (so far always only) criterion in the challenge criteria
            Type = (GameChallengeType)createInfo.Criteria[0].Type,
            PublishDate = now,
            LastUpdateDate = now,
            ExpirationDate = now.AddDays(createInfo.Expires),
        };
        
        this.AddSequentialObject(challenge);

        return challenge;
    }

    public void RemoveChallenge(GameChallenge challenge)
    {
        this.Write(() => {
            // Remove Scores
            this.GameChallengeScores.RemoveRange(s => s.Challenge == challenge);
        
            // Remove Challenge
            this.GameChallenges.Remove(challenge);
        });
    }

    public void RemoveChallengesByUser(GameUser user)
    {
        this.Write(() => {
            // Remove Scores on challenges by the user
            this.GameChallengeScores.RemoveRange(s => s.Challenge.Publisher == user);
        
            // Remove Challenges by user
            this.GameChallenges.RemoveRange(c => c.Publisher == user);
        });
    }

    public void RemoveChallengesForLevel(GameLevel level)
    {
        this.Write(() => {
            // Remove Scores on challenges in the level
            this.GameChallengeScores.RemoveRange(s => s.Challenge.Level == level);

            // Remove Challenges in the level
            this.GameChallenges.RemoveRange(c => c.Level == level);
        });
    }

    public GameChallenge? GetChallengeById(int challengeId)
        => this.GameChallenges.FirstOrDefault(c => c.ChallengeId == challengeId);

    private IEnumerable<GameChallenge> FilterChallengesByStatus(IEnumerable<GameChallenge> challenges, string? status)
    {
        DateTimeOffset now = this._time.Now;
        return status switch
        {
            "active" => challenges.Where(c => c.ExpirationDate > now),
            "expired" => challenges.Where(c => c.ExpirationDate <= now),
            _ => challenges,
        };
    }

    public IEnumerable<GameChallenge> GetChallenges(string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges, filter).AsEnumerable();

    public IEnumerable<GameChallenge> GetChallengesNotByUser(GameUser user, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => c.Publisher != user), filter).AsEnumerable(); 

    public IEnumerable<GameChallenge> GetChallengesByUser(GameUser user, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => c.Publisher == user), filter).AsEnumerable();

    public int GetTotalChallengesByUser(GameUser user, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => c.Publisher == user), filter).Count();

    public IEnumerable<GameChallenge> GetChallengesForLevel(GameLevel level, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => c.Level == level), filter).AsEnumerable();

    public int GetTotalChallengesForLevel(GameLevel level, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => c.Level == level), filter).Count();

    #endregion
    

    #region Score

    public GameChallengeScore CreateChallengeScore(SerializedChallengeAttempt attempt, GameChallenge challenge, GameUser user, int ghostFramesCount)
    {
        DateTimeOffset now = this._time.Now;

        // Get the first score for this challenge
        GameChallengeScore? firstScore = this.GetFirstScoreForChallenge(challenge);
        bool newHighScore = true;

        // Skip this step if there is no first score (and therefore no scores at all) for this challenge yet.
        if (firstScore != null)
        {
            // Get all scores for this challenge by the user whose GhostHash are not null
            IEnumerable<GameChallengeScore> otherScores = 
                this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user && s.GhostHash != null);

            this.Write(() => {
                foreach (GameChallengeScore otherScore in otherScores)
                {
                    // If the current score is not beaten by the new score (new score is lesser than current score),
                    // don't touch the current score's GhostHash and discard the new score's GhostHash later
                    if (attempt.Score < otherScore.Score)
                    {
                        newHighScore = false;
                        continue;
                    }

                    // If the current score is the first score for this challenge, don't touch its GhostHash
                    if (otherScore.Equals(firstScore))
                        continue;

                    otherScore.GhostHash = null;
                }
            });
        }

        // Create new score
        GameChallengeScore score = new()
        {
            Challenge = challenge,
            Publisher = user,
            Score = attempt.Score,
            GhostHash = newHighScore ? attempt.GhostHash : null,
            GhostFramesCount = ghostFramesCount,
            PublishDate = now,
        };

        // Add the score and return it
        this.AddSequentialObject(score);
        
        return score;
    }

    public void RemoveChallengeScore(GameChallengeScore score)
    {
        this.Write(() => 
        {
            this.GameChallengeScores.Remove(score);
        });
        
    }

    public void RemoveChallengeScoresByUser(GameUser user)
    {
        this.Write(() => 
        {
            this.GameChallengeScores.RemoveRange(s => s.Publisher == user);
        });
    }

    public GameChallengeScore? GetFirstScoreForChallenge(GameChallenge challenge)
        => this.GameChallengeScores.FirstOrDefault(s => s.Challenge == challenge && s.GhostHash != null);

    public GameChallengeScoreWithRank? GetRankedFirstScoreForChallenge(GameChallenge challenge)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge && s.GhostHash != null);
        GameChallengeScore? score = scores.FirstOrDefault();

        if (score == null) return null;

        return new(score, scores.OrderByDescending(s => s.Score).ToList().IndexOf(score) + 1);
    }

    public GameChallengeScore? GetNewestScoreForChallengeByUser(GameChallenge challenge, GameUser user)
        => this.GameChallengeScores.LastOrDefault(s => s.Challenge == challenge && s.Publisher == user);

    public GameChallengeScore? GetHighScoreForChallengeByUser(GameChallenge challenge, GameUser user)
        => this.GameChallengeScores.LastOrDefault(s => s.Challenge == challenge && s.Publisher == user && s.GhostHash != null);

    public GameChallengeScoreWithRank? GetRankedHighScoreForChallengeByUser(GameChallenge challenge, GameUser user)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge && s.GhostHash != null);
        GameChallengeScore? score = scores.LastOrDefault(s => s.Publisher.UserId == user.UserId);
        
        if (score == null) return null;

        return new(score, scores.OrderByDescending(s => s.Score).ToList().IndexOf(score) + 1);
    }

    /// <summary>
    /// This method filters out scores which are not the high score of their uploader by keeping scores whose GhostHash is not null.
    /// This also lets the first score of a challenge stay in the list, even if the uploader of that score also has a better score uploaded.
    /// </summary>
    /// <seealso cref="CreateChallengeScore"/>
    private IEnumerable<GameChallengeScore> FilterChallengeScoresByHighScore(IEnumerable<GameChallengeScore> scores)
        => scores.Where(s => s.GhostHash != null);

    public IEnumerable<GameChallengeScore> GetChallengeScoresByUser(GameUser user)
        => this.GameChallengeScores.Where(s => s.Publisher == user).AsEnumerable();

    public IEnumerable<GameChallengeScore> GetScoresForChallenge(GameChallenge challenge, bool highScoresOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge).AsEnumerable();

        if (highScoresOnly) scores = this.FilterChallengeScoresByHighScore(scores);

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public IEnumerable<GameChallengeScoreWithRank> GetRankedScoresForChallenge(GameChallenge challenge, bool highScoresOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge).AsEnumerable();
        if (highScoresOnly) scores = this.FilterChallengeScoresByHighScore(scores);

        IEnumerable<GameChallengeScoreWithRank> rankedScores = scores.Select((s, i) => new GameChallengeScoreWithRank(s, i));

        if(orderByScore) return rankedScores.OrderByDescending(s => s.score.Score);
        return rankedScores;
    }

    public IEnumerable<GameChallengeScore> GetScoresForChallengeByUser(GameChallenge challenge, GameUser user, bool highScoresOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user).AsEnumerable();

        if (highScoresOnly) scores = this.FilterChallengeScoresByHighScore(scores);

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public IEnumerable<GameChallengeScoreWithRank> GetRankedScoresForChallengeByUser(GameChallenge challenge, GameUser user, bool highScoresOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user).AsEnumerable();
        if (highScoresOnly) scores = this.FilterChallengeScoresByHighScore(scores);

        IEnumerable<GameChallengeScoreWithRank> rankedScores = scores.Select((s, i) => new GameChallengeScoreWithRank(s, i));

        if(orderByScore) return rankedScores.OrderByDescending(s => s.score.Score);
        return rankedScores;
    }

    /// <summary>
    /// Returns how often a user has beaten the first score of a challenge (how often they have "cleared" the challenge).
    /// </summary>
    public int GetTotalChallengeClearsByUser(GameChallenge challenge, GameUser user)
    {
        GameChallengeScore? firstScore = this.GetFirstScoreForChallenge(challenge);
        if (firstScore == null) return 0;
        // higher scores and ties both count as cleared/beaten (incase of a score which can't be beaten by a higher score for some reason)
        return this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user && s.Score >= firstScore.Score).Count();
    }

    public IEnumerable<GameChallengeScore> GetScoresForChallengeByUsersMutuals(GameChallenge challenge, GameUser user, bool highScoresOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameUser> mutuals = this.GetUsersMutuals(user);
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge);

        scores = scores.Where(s => mutuals.Contains(s.Publisher));
        if (highScoresOnly) scores = this.FilterChallengeScoresByHighScore(scores);

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public IEnumerable<GameChallengeScoreWithRank> GetRankedScoresForChallengeByUsersMutuals(GameChallenge challenge, GameUser user, bool highScoresOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameUser> mutuals = this.GetUsersMutuals(user);
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge);

        scores = scores.Where(s => mutuals.Contains(s.Publisher));
        if (highScoresOnly) scores = this.FilterChallengeScoresByHighScore(scores);

        IEnumerable<GameChallengeScoreWithRank> rankedScores = scores.Select((s, i) => new GameChallengeScoreWithRank(s, i));

        if(orderByScore) return rankedScores.OrderByDescending(s => s.score.Score);
        return rankedScores;
    }

    /// <seealso cref="GetRankedScoresAroundScore"/>
    public IEnumerable<GameChallengeScoreWithRank> GetRankedHighScoresAroundChallengeScore(GameChallengeScore score, int count, bool highScoresOnly = true)
    {
        if (count <= 0 || count % 2 != 1) 
            throw new ArgumentException("The number of scores must be odd and greater than 0.", nameof(count));

        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == score.Challenge)
            .OrderByDescending(s => s.Score)
            .AsEnumerable();

        if (highScoresOnly) scores = this.FilterChallengeScoresByHighScore(scores);

        // If the given score is the highest score, take the first 3 scores
        if (scores.First().Equals(score))
            return scores.Select((s, i) => new GameChallengeScoreWithRank(s, i + 1)).Take(count);

        // If the given score is the lowest score, take the last 3 scores
        else if (scores.Last().Equals(score))
            return scores.Select((s, i) => new GameChallengeScoreWithRank(s, i + 1)).TakeLast(count);

        // Else return the given score with other scores around it
        else
            return scores.Select((s, i) => new GameChallengeScoreWithRank(s, i + 1))
                .Skip(Math.Min(scores.Count(), scores.ToList().IndexOf(score) - count / 2)) // center user's score around other scores
                .Take(count)
                .AsEnumerable();
    }

    #endregion

}