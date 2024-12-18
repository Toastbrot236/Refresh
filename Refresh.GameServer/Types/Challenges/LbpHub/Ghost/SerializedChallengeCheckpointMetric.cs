using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub.Ghost;

[XmlRoot("metric")]
[XmlType("metric")]
public class SerializedChallengeCheckpointMetric : IDataConvertableFrom<SerializedChallengeCheckpointMetric, GameChallengeCheckpointMetric>
{
    [XmlAttribute("id")] public int Id { get; set; }
    [XmlText] public long Value { get; set; }

    public static SerializedChallengeCheckpointMetric? FromOld(GameChallengeCheckpointMetric? old, DataContext dataContext)
    {
        if (old == null)
            return null;

        return new SerializedChallengeCheckpointMetric
        {
            Id = old.Id,
            Value = old.Value,
        }; 
    }

    public static IEnumerable<SerializedChallengeCheckpointMetric> FromOldList(IEnumerable<GameChallengeCheckpointMetric> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);
}