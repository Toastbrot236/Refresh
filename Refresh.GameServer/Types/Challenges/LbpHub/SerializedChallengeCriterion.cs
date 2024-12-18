using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("criterion")]
[XmlType("criterion")]
public class SerializedChallengeCriterion : IDataConvertableFrom<SerializedChallengeCriterion, GameChallengeCriterion>
{
    [XmlAttribute("name")] public byte Type { get; set; } = 0;
    [XmlText] public long Value { get; set; }

    public static SerializedChallengeCriterion? FromOld(GameChallengeCriterion? old, DataContext dataContext)
    {
        if (old == null)
            return null;

        return new SerializedChallengeCriterion
        {
            Type = (byte)old.Type,
            Value = old.Value,
        };
    }

    public static IEnumerable<SerializedChallengeCriterion> FromOldList(IEnumerable<GameChallengeCriterion> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);
}