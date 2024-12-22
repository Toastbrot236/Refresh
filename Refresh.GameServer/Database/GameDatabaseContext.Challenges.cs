using Refresh.GameServer.Types.Challenges.LbpHub;
using Refresh.GameServer.Types.Challenges.LbpHub.Ghost;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Database;

public partial class GameDatabaseContext // Challenges
{
    #region Challenge
    public GameChallenge CreateChallenge(SerializedChallenge createInfo, GameLevel level, GameUser user)
    {
        DateTimeOffset now = DateTimeOffset.Now;

        // Create challenge
        GameChallenge challenge = new()
        {
            Name = createInfo.Name,
            Publisher = user,
            Level = level,
            CreationDate = SerializedChallenge.ToDateTimeOffset(createInfo.Published),
            PublishDate = now,
            LastUpdateDate = now,
            ExpirationDate = SerializedChallenge.ToDateTimeOffset(createInfo.Expires),
        };
        
        this.AddSequentialObject(challenge);

        // Create criteria of challenge
        this.Write(() => 
        {
            foreach(SerializedChallengeCriterion Criterion in createInfo.Criteria)
            {
                this.GameChallengeCriteria.Add(new GameChallengeCriterion
                {
                    Type = (GameChallengeType)Criterion.Type,
                    Value = Criterion.Value,
                    Challenge = challenge,
                });
            }
        });

        return challenge;
    }

    public void RemoveChallenge(GameChallenge challenge)
    {
        this.Write(() => {
            // Remove Scores
            this.GameChallengeScores.RemoveRange(s => s.Challenge == challenge);

            // Remove Criteria
            this.GameChallengeCriteria.RemoveRange(c => c.Challenge == challenge);
        
            // Remove Challenge
            this.GameChallenges.Remove(challenge);
        });
    }

    public void RemoveChallengesByUser(GameUser user)
    {
        
    }

    public void RemoveChallengesForLevel(GameLevel level)
    {
        
    }

    private IEnumerable<GameChallenge> FilterChallenges(IEnumerable<GameChallenge> challenges, string? filter)
    {
        if (filter == null) return challenges;

        DateTimeOffset now = DateTimeOffset.Now;
        switch (filter)
        {
            case "active":
                return challenges.Where(c => c.ExpirationDate > now);
            case "expired":
                return challenges.Where(c => c.ExpirationDate <= now);
            default:
                return challenges;
        }
    }

    public GameChallenge? GetChallengeById(int challengeId)
        => this.GameChallenges.FirstOrDefault(c => c.ChallengeId == challengeId);

    public IEnumerable<GameChallenge> GetChallengesByUser(GameUser user, string? filter = null)
    {
        IEnumerable<GameChallenge> challenges = this.GameChallenges.Where(c => c.Publisher == user).AsEnumerable();
        return this.FilterChallenges(challenges, filter);
    } 

    public int GetTotalChallengesByUser(GameUser user, string? filter = null)
        => this.GetChallengesByUser(user, filter).Count();

    public IEnumerable<GameChallenge> GetChallengesForLevel(GameLevel level, string? filter = null)
    {
        IEnumerable<GameChallenge> challenges = this.GameChallenges.Where(c => c.Level == level).AsEnumerable();
        return this.FilterChallenges(challenges, filter);
    }

    public int GetTotalChallengesForLevel(GameLevel level, string? filter = null)
        => this.GetChallengesForLevel(level, filter).Count();

    #endregion

    #region Criterion
    public IEnumerable<GameChallengeCriterion> GetChallengeCriteria(GameChallenge challenge)
        => this.GameChallengeCriteria.Where(c => c.Challenge == challenge).AsEnumerable();
    public IEnumerable<GameChallengeCriterion> GetChallengeCriteriaOfType(GameChallenge challenge, GameChallengeType type)
        => this.GameChallengeCriteria.Where(c => c.Challenge == challenge && c.Type == type).AsEnumerable();
    
    #endregion

    #region Score

    public GameChallengeScore CreateChallengeScore(SerializedChallengeAttempt attempt, GameChallenge challenge, GameUser user)
    {
        DateTimeOffset now = DateTimeOffset.Now;

        // Create new score
        GameChallengeScore score = new()
        {
            Challenge = challenge,
            Publisher = user,
            Score = attempt.Score,
            GhostDataHash = attempt.GhostDataHash,
            PublishDate = now,
        };

        this.Write(() => 
        {
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
        IEnumerable<GameChallengeScore> scores = this.GetChallengeScoresByUser(user);

        // Remove the scores
        foreach(GameChallengeScore score in scores)
        {
            this.RemoveChallengeScore(score);
        }
    }

    public GameChallengeScore SetOriginalScoreForChallenge(GameChallenge challenge)
        => this.GameChallenges.First(c => c == challenge).OriginalScore;

    public IEnumerable<GameChallengeScore> GetChallengeScoresByUser(GameUser user, bool orderByScore = false)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Publisher == user).AsEnumerable();

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public IEnumerable<GameChallengeScore> GetChallengeScoresForChallenge(GameChallenge challenge, bool orderByScore = false)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge).AsEnumerable();

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public int GetTotalChallengeScoresForChallenge(GameChallenge challenge, bool orderByScore = false)
        => this.GameChallengeScores.Where(s => s.Challenge == challenge).Count();
    
    public IEnumerable<GameChallengeScore> GetChallengeScoresForChallengeByUser(GameChallenge challenge, GameUser user, bool orderByScore = false)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(s => s.Challenge == challenge && s.Publisher == user).AsEnumerable();

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    #endregion
}