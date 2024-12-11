using System.Xml.Serialization;
using Refresh.Common.Constants;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("challenge-score")]
[XmlType("challenge-score")]
public class SerializedChallengeScore
{
    [XmlElement("ghost")] public string Ghost { get; set; } = SystemUsers.UnknownUserName;
    [XmlElement("player")] public string Publisher { get; set; } = SystemUsers.DeletedUserName;
    [XmlElement("rank")] public int Rank { get; set; }
    [XmlElement("score")] public long Score { get; set; }

    public static SerializedChallengeScore FromOld(GameChallengeScore score, int rank)
    {
        return new SerializedChallengeScore
        {
            Ghost = score.Ghost,
            Publisher = score.Publisher.Username,
            Rank = rank,
            Score = score.Score,
        };
    }
}