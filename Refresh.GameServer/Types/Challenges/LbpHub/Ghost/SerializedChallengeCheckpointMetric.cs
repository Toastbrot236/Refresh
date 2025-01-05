using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("metric")]
[XmlType("metric")]
public class SerializedChallengeCheckpointMetric
{
    // not needed
    /*
    [XmlAttribute("id")] public int Id { get; set; }
    [XmlText] public long Value { get; set; }
    */
}