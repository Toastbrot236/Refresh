using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("ghost_frame")]
[XmlType("ghost_frame")]
public class SerializedChallengeGhostFrame : IDataConvertableFrom<SerializedChallengeGhostFrame, GameChallengeGhostFrame>
{
    [XmlAttribute("time")] public long Time { get; set; }
    [XmlAttribute("X")] public int LocationX { get; set; }
    [XmlAttribute("Y")] public int LocationY { get; set; }
    [XmlAttribute("Z")] public int LocationZ { get; set; }
    [XmlAttribute("rotation")] public float Rotation { get; set; }
    [XmlAttribute("keyframe")] public bool Keyframe { get; set; }

    public static SerializedChallengeGhostFrame? FromOld(GameChallengeGhostFrame? old, DataContext dataContext)
    {
        if (old == null)
            return null;
        
        return new SerializedChallengeGhostFrame
        {
            Time = old.Time,
            LocationX = old.LocationX,
            LocationY = old.LocationY,
            LocationZ = old.LocationZ,
            Rotation = old.Rotation,
            Keyframe = old.Keyframe,
        };
    }

    public static IEnumerable<SerializedChallengeGhostFrame> FromOldList(IEnumerable<GameChallengeGhostFrame> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);
}