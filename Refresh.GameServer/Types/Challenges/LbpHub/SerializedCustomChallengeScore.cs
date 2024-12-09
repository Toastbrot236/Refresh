using System.Xml.Serialization;
using Refresh.Common.Constants;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("challenge-score")]
[XmlType("challenge-score")]
public class SerializedCustomChallengeScore
{
    [XmlElement("ghost")] public string Ghost { get; set; } = SystemUsers.UnknownUserName;
    [XmlElement("player")] public string Player { get; set; } = SystemUsers.DeletedUserName;
    [XmlElement("rank")] public int Rank { get; set; }
    [XmlElement("score")] public long Score { get; set; }

    public static SerializedCustomChallengeScore FromGameChallengeScore(GameCustomChallengeScore score, int rank)
    {
        return new SerializedCustomChallengeScore
        {
            Ghost = score.Ghost,
            Player = score.Player.Username,
            Rank = rank,
            Score = score.Score,
        };
    }
}