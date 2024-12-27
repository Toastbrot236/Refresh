using System.Xml.Serialization;
using Refresh.Common.Constants;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("challenge-score")]
[XmlType("challenge-score")]
public class SerializedChallengeScore : SerializedChallengeAttempt, IDataConvertableFrom<SerializedChallengeScore, GameChallengeScore>
{
    [XmlElement("player")] public string PublisherName { get; set; } = SystemUsers.UnknownUserName;
    [XmlElement("rank")] public int Rank { get; set; }

    public static SerializedChallengeScore? FromOld(GameChallengeScore? old, DataContext dataContext)
        => FromOld(old, 0);

    public static SerializedChallengeScore? FromOld(GameChallengeScore? old, int rank)
    {
        if (old == null)
            return null;

        return new SerializedChallengeScore
        {
            GhostHash = old.GhostHash,
            Score = old.Score,
            PublisherName = old.Publisher.Username,
            Rank = rank,
        };
    }

    public static IEnumerable<SerializedChallengeScore> FromOldList(IEnumerable<GameChallengeScore> oldList, DataContext dataContext)
        => oldList.Select((s, i) => FromOld(s, i + 1)!);
}