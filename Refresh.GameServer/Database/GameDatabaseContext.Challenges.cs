using Refresh.GameServer.Types.Challenges.LbpHub;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Database;

public partial class GameDatabaseContext // Challenges
{
    public GameCustomChallenge CreateChallenge(SerializedCustomChallenge createInfo, GameLevel level, GameUser user)
    {
        GameCustomChallenge challenge = new()
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

        this.Write(() => {
            this.AddSequentialObject(challenge);
        });

        return challenge;
    }

    public GameCustomChallenge? GetChallengeById(int challengeId)
        => this.GameCustomChallenges.FirstOrDefault(c => c.ChallengeId == challengeId);

    public IEnumerable<GameCustomChallenge> GetChallengesByUser(GameUser user)
        => this.GameCustomChallenges.Where(c => c.Publisher.UserId == user.UserId).AsEnumerable()
            .OrderBy(c => c.PublishDate);
}