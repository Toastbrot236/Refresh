using Org.BouncyCastle.Asn1.Cmp;
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
        // Create challenge
        GameChallenge challenge = new()
        {
            Name = createInfo.Name,
            Publisher = user,
            Level = level,
            PublishDate = DateTimeOffset.FromUnixTimeMilliseconds(createInfo.Published),
            ExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds(createInfo.Expiration),
        };
        
        this.AddSequentialObject(challenge);

        // Create criteria of challenge
        foreach(SerializedChallengeCriterion Criterion in createInfo.Criteria)
        {
            this.GameChallengeCriteria.Add(new GameChallengeCriterion
            {
                _Type = Criterion.Type,
                Value = Criterion.Value,
                Challenge = challenge,
            });
        }

        // Create original Score
        this.CreateChallengeScore(createInfo.Score, challenge, user);

        return challenge;
    }

    public void RemoveChallenge(GameChallenge challenge)
    {
        // Remove all other Challenge relations
        this.GameChallengeCheckpointMetrics.RemoveRange(m => m.Checkpoint.Score.Challenge == challenge);
        this.GameChallengeCheckpoints.RemoveRange(c => c.Score.Challenge == challenge);
        this.GameChallengeGhostFrames.RemoveRange(f => f.Score.Challenge == challenge);

        // Remove Scores
        this.GameChallengeScores.RemoveRange(s => s.Challenge == challenge);

        // Remove Criteria
        this.GameChallengeCriteria.RemoveRange(c => c.Challenge == challenge);
        
        // Remove Challenge
        this.GameChallenges.Remove(challenge);
    }

    public void ArchiveChallenge(GameChallenge challenge, bool archive)
    {
        this.Write(() => 
        {
            challenge.Archived = archive;
        });
    }

    public GameChallenge? GetChallengeById(int challengeId)
        => this.GameChallenges.FirstOrDefault(c => c.ChallengeId == challengeId);

    public IEnumerable<GameChallenge> GetChallengesByUser(GameUser user)
        => this.GameChallenges.Where(c => c.Publisher.UserId == user.UserId).AsEnumerable();
    public int GetTotalChallengesByUser(GameUser user)
        => this.GameChallenges.Where(c => c.Publisher.UserId == user.UserId).Count();

    public IEnumerable<GameChallenge> GetChallengesForLevel(GameLevel level)
        => this.GameChallenges.Where(c => c.Level == level).AsEnumerable();
    public int GetTotalChallengesForLevel(GameLevel level)
        => this.GameChallenges.Where(c => c.Level == level).Count();

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
        // Create new score
        GameChallengeScore score = new()
        {
            Ghost = attempt.Ghost,
            Publisher = user,
            Challenge = challenge,
            Score = attempt.Score,
            OriginalScore = false,
        };

        this.GameChallengeScores.Add(score);

        return score;
    }
    public GameChallengeScore CreateChallengeScore(long scoreValue, GameChallenge challenge, GameUser user)
    {
        // Create new score
        GameChallengeScore score = new()
        {
            Ghost = user.Username,
            Publisher = user,
            Challenge = challenge,
            Score = scoreValue,
            OriginalScore = true,
        };

        this.GameChallengeScores.Add(score);

        return score;
    }

    public void RemoveChallengeScore(GameChallengeScore score)
    {
        // Remove score relations
        this.GameChallengeCheckpointMetrics.RemoveRange(m => m.Checkpoint.Score == score);
        this.GameChallengeCheckpoints.RemoveRange(c => c.Score == score);
        this.GameChallengeGhostFrames.RemoveRange(f => f.Score == score);
        
        // Remove score
        this.GameChallengeScores.Remove(score);
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

    public GameChallengeScore GetOriginalChallengeScoreForChallenge(GameChallenge challenge)
        => this.GameChallengeScores.FirstOrDefault(c => c.Challenge == challenge && c.OriginalScore)!;

    public IEnumerable<GameChallengeScore> GetChallengeScoresByUser(GameUser user, bool orderByScore = false)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(c => c.Publisher == user).AsEnumerable();

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    public IEnumerable<GameChallengeScore> GetChallengeScoresForChallenge(GameChallenge challenge, bool orderByScore = false)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(c => c.Challenge == challenge).AsEnumerable();

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }
    
    public IEnumerable<GameChallengeScore> GetChallengeScoresForChallengeByUser(GameChallenge challenge, GameUser user, bool orderByScore = false)
    {
        IEnumerable<GameChallengeScore> scores = this.GameChallengeScores.Where(c => c.Challenge == challenge && c.Publisher == user).AsEnumerable();

        if(orderByScore) return scores.OrderByDescending(s => s.Score);
        return scores;
    }

    #endregion


    #region Ghost Frame
    public IEnumerable<GameChallengeGhostFrame> GetChallengeGhostFramesOfScore(GameChallengeScore score)
        => this.GameChallengeGhostFrames.Where(c => c.Score == score).AsEnumerable();
    
    #endregion


    #region Checkpoint
    public IEnumerable<GameChallengeCheckpoint> GetChallengeCheckpointsOfScore(GameChallengeScore score)
        => this.GameChallengeCheckpoints.Where(c => c.Score == score).AsEnumerable();
    public int GetTotalChallengeCheckpointsOfScore(GameChallengeScore score)
        => this.GameChallengeCheckpoints.Where(c => c.Score == score).Count();

    public IEnumerable<GameChallengeCheckpoint> GetChallengeCheckpointsByUidInLevel(int uid, GameLevel level)
        => this.GameChallengeCheckpoints.Where(c => c.Uid == uid && c.Score.Challenge.Level == level).AsEnumerable();

    public GameChallengeCheckpoint? GetStartCheckpointOfChallengeScore(GameChallengeScore score)
        => this.GameChallengeCheckpoints.First(c => c.Score == score && c.start);
    public GameChallengeCheckpoint? GetFinishCheckpointOfChallengeScore(GameChallengeScore score)
        => this.GameChallengeCheckpoints.First(c => c.Score == score && c.finish);

    #endregion


    #region Checkpoint Metric
    public IEnumerable<GameChallengeCheckpointMetric> GetChallengeCheckpointMetrics(GameChallengeCheckpoint checkpoint)
        => this.GameChallengeCheckpointMetrics.Where(m => m.Checkpoint == checkpoint).AsEnumerable();

    #endregion
}