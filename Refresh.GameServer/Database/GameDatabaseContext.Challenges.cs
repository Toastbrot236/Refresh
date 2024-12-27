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

        // Create challenge
        GameChallenge challenge = new()
        {
            Name = createInfo.Name,
            Publisher = user,
            Level = level,
            StartCheckpointUid = createInfo.StartCheckpointUid,
            EndCheckpointUid = createInfo.EndCheckpointUid,
            Type = (GameChallengeType)createInfo.Criteria[0].Type,
            PublishDate = now,
            LastUpdateDate = now,
            ExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds
            (
                now.ToUnixTimeMilliseconds() + 
                SerializedChallenge.ToUnixMilliseconds(createInfo.Expires)
            ),
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
        long nowInMilliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        return status switch
        {
            "active" => challenges.Where(c => c.ExpirationDate.ToUnixTimeMilliseconds() > nowInMilliseconds),
            "expired" => challenges.Where(c => c.ExpirationDate.ToUnixTimeMilliseconds() <= nowInMilliseconds),
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

    public IEnumerable<GameChallenge> GetChallengesByUsersMutuals(GameUser user, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => this.GetUsersMutuals(user).Contains(c.Publisher)), filter).AsEnumerable();

    public int GetTotalChallengesByUsersMutuals(GameUser user, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => this.GetUsersMutuals(user).Contains(c.Publisher)), filter).Count();

    public IEnumerable<GameChallenge> GetChallengesForLevel(GameLevel level, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => c.Level == level), filter).AsEnumerable();

    public int GetTotalChallengesForLevel(GameLevel level, string? filter = null)
        => this.FilterChallengesByStatus(this.GameChallenges.Where(c => c.Level == level), filter).Count();

    #endregion
    

    #region Score

    public GameChallengeScore CreateChallengeScore(SerializedChallengeAttempt attempt, GameChallenge challenge, GameUser user)
    {
        DateTimeOffset now = this._time.Now;

        // Get the first score for this challenge
        GameChallengeScore? firstScore = this.GetFirstScoreForChallenge(challenge);
        bool newPersonalBest = true;

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
                    // don't touch the current score's GhostHash and discard the new score's GhostHash later using newPersonalBest
                    if (attempt.Score < otherScore.Score)
                    {
                        newPersonalBest = false;
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
            GhostHash = newPersonalBest ? attempt.GhostHash : null,
            PublishDate = now,
        };

        this.Write(() => 
        {
            // Add the new score
            this.GameChallengeScores.Add(score);
        });
        
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

    public GameChallengeScore? GetNewestScoreForChallengeByUser(GameChallenge challenge, GameUser user)
        => this.GameChallengeScores.LastOrDefault(s => s.Challenge == challenge && s.Publisher == user);

    public IEnumerable<GameChallengeScore> GetChallengeScoresByUser(GameUser user)
        => this.GameChallengeScores.Where(s => s.Publisher == user).AsEnumerable();

    public int GetTotalScoresByUser(GameUser user)
        => this.GameChallengeScores.Where(s => s.Publisher == user).Count();

    /// <summary>
    /// This method filters scores out of an IEnumerable of GameChallengeScores which are not the personal bests of their uploader by 
    /// filtering out scores whose GhostHash references are null. This also lets the first score of a challenge stay in the list, even if
    /// the uploader of said score also has a better score uploaded.
    /// </summary>
    /// <seealso cref="CreateChallengeScore"/>
    private IEnumerable<GameChallengeScore> FilterChallengeScoresByPersonalBest(IEnumerable<GameChallengeScore> scores)
        => scores.Where(s => s.GhostHash != null);

    public IEnumerable<GameChallengeScore> GetScoresForChallenge(GameChallenge challenge, bool personalBestsOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge).AsEnumerable();

        if (personalBestsOnly) scores = this.FilterChallengeScoresByPersonalBest(scores);

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public int GetTotalScoresForChallenge(GameChallenge challenge)
        => this.GameChallengeScores.Where(s => s.Challenge == challenge).Count();

    public IEnumerable<GameChallengeScore> GetScoresForChallengeByUser(GameChallenge challenge, GameUser user, bool personalBestsOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user).AsEnumerable();

        if (personalBestsOnly) scores = this.FilterChallengeScoresByPersonalBest(scores);

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public int GetTotalScoresForChallengeByUser(GameChallenge challenge, GameUser user)
        => this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user).Count();

    public IEnumerable<GameChallengeScore> GetScoresForChallengeByUsersMutuals(GameChallenge challenge, GameUser user, bool personalBestsOnly = true, bool orderByScore = true)
    {
        IEnumerable<GameUser> mutuals = this.GetUsersMutuals(user);
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge);
        scores = scores.Where(s => mutuals.Contains(s.Publisher));

        if (personalBestsOnly) scores = this.FilterChallengeScoresByPersonalBest(scores);

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public IEnumerable<SerializedChallengeScore> GetScoresAroundChallengeScore(GameChallengeScore score, int count, bool personalBestsOnly = true)
    {
        if (count <= 0 || count % 2 != 1) 
            throw new ArgumentException("The number of scores must be odd and greater than 0.", nameof(count));

        List<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == score.Challenge)
            .OrderByDescending(s => s.Score)
            .ToList();

        if (personalBestsOnly) scores = this.FilterChallengeScoresByPersonalBest(scores)
            .ToList();

        return scores.Select((s, i) => SerializedChallengeScore.FromOld(s, i + 1)!)
            .Skip(Math.Min(scores.Count, scores.IndexOf(score) - count / 2)) // center user's score around other scores
            .Take(count)
            .AsEnumerable();
    }

    #endregion
}