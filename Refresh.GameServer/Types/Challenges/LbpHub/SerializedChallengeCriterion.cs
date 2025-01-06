using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("criterion")]
[XmlType("criterion")]
public class SerializedChallengeCriterion : IDataConvertableFrom<SerializedChallengeCriterion, GameChallenge>
{
    [XmlAttribute("name")] public byte Type { get; set; } = 0;

    /// <summary>
    /// Appears to always be 0 when sent by the game, therefore we don't need to save it.
    /// </summary>
    [XmlText] public long Value { get; set; }

    public static SerializedChallengeCriterion? FromOld(GameChallenge? old, DataContext dataContext)
    {
        if (old == null)
            return null;

        return new SerializedChallengeCriterion
        {
            Type = (byte)old.Type,
            Value = 0,
        };
    }

    public static IEnumerable<SerializedChallengeCriterion> FromOldList(IEnumerable<GameChallenge> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);
}