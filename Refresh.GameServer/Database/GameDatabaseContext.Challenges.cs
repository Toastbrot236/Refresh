using Refresh.GameServer.Types.Challenges.LbpHub;
using Refresh.GameServer.Types.Challenges.LbpHub.Ghost;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Database;

public partial class GameDatabaseContext // Challenges
{
    public GameChallenge CreateChallenge(SerializedChallenge createInfo, GameLevel level, GameUser user)
    {
        // Create and Add challenge
        GameChallenge Challenge = new()
        {
            Name = createInfo.Name,
            Publisher = user,
            Level = level,
            Score = createInfo.Score,
            StartCheckpointId = createInfo.StartCheckpointId,
            EndCheckpointId = createInfo.EndCheckpointId,
            PublishDate = DateTimeOffset.FromUnixTimeMilliseconds(createInfo.Published),
            ExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds(createInfo.Expiration),
        };
        
        this.AddSequentialObject(Challenge);

        // Create and Add criteria of challenge
        IEnumerable<GameChallengeCriterion> Criteria = [];

        foreach(SerializedChallengeCriterion Criterion in createInfo.Criteria)
        {
            this.GameChallengeCriterions.Add(new GameChallengeCriterion
            {
                _Type = Criterion.Type,
                Value = Criterion.Value,
                Challenge = Challenge,
            });
        }

        return Challenge;
    }

    public void RemoveChallenge(GameChallenge challenge)
    {
        // TODO:
        // Remove Challenge
        // Remove Criteria
        // Remove Scores and other relations
    }

    public GameChallengeScore CreateChallengeScore(SerializedChallengeAttempt attempt, GameChallenge challenge, GameUser user)
    {
        // Create new score
        GameChallengeScore score = new()
        {
            Ghost = attempt.Ghost,
            Publisher = user,
            Challenge = challenge,
            Score = attempt.Score,
        };

        this.GameChallengeScores.Add(score);

        return score;
    }

    public void RemoveChallengeScore(GameChallengeScore score)
    {
        // TODO: Remove score
    }

    public GameChallenge? GetChallengeById(int challengeId)
        => this.GameChallenges.FirstOrDefault(c => c.ChallengeId == challengeId);

    public IEnumerable<GameChallenge> GetChallengesByUser(GameUser user)
        => this.GameChallenges.Where(c => c.Publisher.UserId == user.UserId).AsEnumerable();
    public IEnumerable<GameChallenge> GetChallengesForLevel(GameLevel level)
        => this.GameChallenges.Where(c => c.Level == level).AsEnumerable();

    public IEnumerable<GameChallengeCriterion> GetChallengeCriteria(GameChallenge challenge)
        => this.GameChallengeCriterions.Where(c => c.Challenge == challenge).AsEnumerable();
    
    public IEnumerable<GameChallengeScore> GetChallengeScoresByUser(GameChallenge challenge)
        => this.GameChallengeScores.Where(c => c.Challenge == challenge).AsEnumerable();
    public IEnumerable<GameChallengeScore> GetChallengeScoresForChallenge(GameChallenge challenge)
        => this.GameChallengeScores.Where(c => c.Challenge == challenge).AsEnumerable();

    public IEnumerable<GameChallengeCheckpoint> GetChallengeCheckpointsForScore(GameChallengeScore score)
        => this.GameChallengeCheckpoints.Where(c => c.Score == score).AsEnumerable();
    public IEnumerable<GameChallengeGhostFrame> GetChallengeGhostFramesByScore(GameChallengeScore score)
        => this.GameChallengeGhostFrames.Where(c => c.Score == score).AsEnumerable();

    public IEnumerable<GameChallengeGhostFrame> GetChallengeGhostFramesForScore(GameChallengeScore score)
        => this.GameChallengeGhostFrames.Where(c => c.Score == score).AsEnumerable();

}