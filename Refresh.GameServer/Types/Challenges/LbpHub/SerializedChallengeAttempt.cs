using System.Xml.Serialization;
using Refresh.Common.Constants;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("challenge-attempt")]
[XmlType("challenge-attempt")]
public class SerializedChallengeAttempt
{
    [XmlElement("score")] public long Score { get; set; }
    [XmlElement("ghost")] public string Ghost { get; set; } = SystemUsers.UnknownUserName;
}