using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("metric")]
[XmlType("metric")]
public class SerializedChallengeCheckpointMetric
{
    // usually Id is an array of numbers and Value is a number, but we dont need to use these values
    [XmlAttribute("id")] public List<string> Id { get; set; } = [];
    [XmlText] public string? Value { get; set; }
}