using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("checkpoint")]
[XmlType("checkpoint")]
public class SerializedChallengeCheckpoint : IDataConvertableFrom<SerializedChallengeCheckpoint, GameChallengeCheckpoint>
{
    [XmlAttribute("uid")] public int Uid { get; set; }
    [XmlAttribute("time")] public long Time { get; set; }
    [XmlAttribute("metric")] public List<SerializedChallengeCheckpointMetric> Metrics { get; set; } = [];

    public static SerializedChallengeCheckpoint? FromOld(GameChallengeCheckpoint? old, DataContext dataContext)
    {
        if (old == null)
            return null;

        return new SerializedChallengeCheckpoint
        {
            Uid = old.Uid,
            Time = old.Time,
        }; 
    }

    public static IEnumerable<SerializedChallengeCheckpoint> FromOldList(IEnumerable<GameChallengeCheckpoint> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);
}