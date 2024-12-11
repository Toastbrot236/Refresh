using Refresh.GameServer.Types.Challenges.LbpHub;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Database;

public partial class GameDatabaseContext // Challenges
{
    public GameChallenge CreateChallenge(SerializedChallenge createInfo, GameLevel level, GameUser user)
    {
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

        IEnumerable<GameChallengeCriterion> Criteria = [];
        
        this.Write(() => {
            // Add challenge
            this.AddSequentialObject(Challenge);
            
            // Add criteria of challenge
            foreach(SerializedChallengeCriterion Criterion in createInfo.Criteria)
            {
                this.GameChallengeCriterions.Add(new GameChallengeCriterion
                {
                    _Type = Criterion.Type,
                    Value = Criterion.Value,
                    Challenge = Challenge,
                });
            }
        });

        return Challenge;
    }

    public GameChallengeScore CreateChallengeScore(SerializedChallengeAttempt attempt, GameChallenge challenge, GameUser user)
    {
        this.GameChallengeScores.RemoveRange(s => s.Publisher.UserId == user.UserId && s.Challenge == challenge);

        GameChallengeScore Score = new()
        {
            Ghost = attempt.Ghost,
            Publisher = user,
            Challenge = challenge,
            Score = attempt.Score,
        };

        this.Write(() =>
        {
            this.GameChallengeScores.Add(Score);
        });

        return Score;
    }

    public void RemoveChallenge(GameChallenge challenge)
    {

    }

    public GameChallenge? GetChallengeById(int challengeId)
        => this.GameChallenges.FirstOrDefault(c => c.ChallengeId == challengeId);
    public IEnumerable<GameChallenge> GetChallengesByUser(GameUser user)
        => this.GameChallenges.Where(c => c.Publisher.UserId == user.UserId).AsEnumerable();
    public IEnumerable<GameChallenge> GetChallengesForLevel(GameLevel level)
        => this.GameChallenges.Where(c => c.Level == level).AsEnumerable();
    public IEnumerable<GameChallengeScore> GetChallengeScoresForChallenge(GameChallenge challenge)
        => this.GameChallengeScores.Where(c => c.Challenge == challenge).AsEnumerable();
}