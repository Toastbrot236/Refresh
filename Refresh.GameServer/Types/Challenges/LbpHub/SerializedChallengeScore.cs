using System.Xml.Serialization;
using Refresh.Common.Constants;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("challenge-score")]
[XmlType("challenge-score")]
public class SerializedChallengeScore : SerializedChallengeAttempt, IDataConvertableFrom<SerializedChallengeScore, GameChallengeScore>
{
    [XmlElement("player")] public string Player { get; set; } = SystemUsers.UnknownUserName;
    [XmlElement("rank")] public int Rank { get; set; }

    public static SerializedChallengeScore? FromOld(GameChallengeScore? old, DataContext dataContext)
    {
        if (old == null)
            return null;

        return new SerializedChallengeScore
        {
            GhostDataHash = old.GhostDataHash,
            Score = old.Score,
            Player = old.Publisher.Username,
            Rank = 7,
        };
    }

    public static IEnumerable<SerializedChallengeScore> FromOldList(IEnumerable<GameChallengeScore> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);
}