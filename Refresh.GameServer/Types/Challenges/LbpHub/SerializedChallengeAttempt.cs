using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

//  <challenge-attempt>
//      <score>9223371991757619189</score>
//      <ghost>b8127d2b7acb3062249022bbce63e81e4f996c52</ghost>
//  </challenge-attempt>
[XmlRoot("challenge-attempt")]
[XmlType("challenge-attempt")]
public class SerializedChallengeAttempt
{
    [XmlElement("score")] public long Score { get; set; }
    [XmlElement("ghost")] public string GhostHash { get; set; } = "";
}