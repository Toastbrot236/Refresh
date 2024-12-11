using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("criterion")]
[XmlType("criterion")]
public class SerializedChallengeCriterion
{
    [XmlAttribute("name")] public byte Type { get; set; } = 0;
    [XmlText] public long Value { get; set; }
}