using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Photos;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("challenge")]
[XmlType("challenge")]
public class SerializedChallenge : IDataConvertableFrom<SerializedChallenge, GameChallenge>
{
    [XmlElement("id")] public int ChallengeId { get; set; }
    [XmlElement("slot")] public SerializedPhotoLevel Level { get; set; }
    [XmlElement("name")] public string Name { get; set; } = string.Empty;
    [XmlElement("author")] public string PublisherUsername { get; set; } = string.Empty;
    [XmlElement("score")] public long Score { get; set; }
    [XmlElement("start-checkpoint")] public int StartCheckpointId { get; set; }
    [XmlElement("end-checkpoint")] public int EndCheckpointId { get; set; }
    [XmlElement("published")] public long Published { get; set; }
    [XmlElement("expires")] public long Expiration { get; set; }
    [XmlArray("criteria")] public List<SerializedChallengeCriterion> Criteria { get; set; } = [];


    public static SerializedChallenge? FromOld(GameChallenge? old, DataContext dataContext)
    {
        if (old == null)
            return null;

        return new SerializedChallenge
        {
            ChallengeId = old.ChallengeId,
        };
    }

    public static IEnumerable<SerializedChallenge> FromOldList(IEnumerable<GameChallenge> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);
}