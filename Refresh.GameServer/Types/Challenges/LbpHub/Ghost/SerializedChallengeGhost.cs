using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

// There are two classes for what could have easily been one class for optimization reasons,
// see Refresh.GameServer.Endpoints.Game.FixupScoreAndGhost

[XmlRoot("ghost")]
[XmlType("ghost")]
public class SerializedChallengeGhostCheckpoints
{
    [XmlArray("checkpoint")] public List<SerializedChallengeCheckpoint> Checkpoints { get; set; } = [];
}

[XmlRoot("ghost")]
[XmlType("ghost")]
public class SerializedChallengeGhostFrames
{
    [XmlArray("ghost_frame")] public List<SerializedChallengeGhostFrame> Frames { get; set; } = [];
}