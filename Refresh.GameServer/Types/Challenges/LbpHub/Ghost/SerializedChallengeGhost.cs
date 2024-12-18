using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("ghost")]
[XmlType("ghost")]
public class SerializedChallengeGhost : IDataConvertableFrom<SerializedChallengeGhost, GameChallengeScore>
{
    [XmlElement("checkpoint")] public List<SerializedChallengeCheckpoint> Checkpoints { get; set; } = [];
    [XmlElement("ghost_frame")] public List<SerializedChallengeGhostFrame> Frames { get; set; } = [];
    
    public static SerializedChallengeGhost? FromOld(GameChallengeScore? old, DataContext dataContext)
    {
        if (old == null)
            return null;

        return new SerializedChallengeGhost
        {
            Checkpoints = dataContext.Database.GetCheckpointsForChallengeScore(old),
            Frames = dataContext.Database.GetChallengeGhostFrames(old),
        }; 
    }

    public static IEnumerable<SerializedChallengeGhost> FromOldList(IEnumerable<GameChallengeScore> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);
}