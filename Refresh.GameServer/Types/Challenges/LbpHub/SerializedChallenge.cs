using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.Photos;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("challenge")]
[XmlType("challenge")]
public class SerializedChallenge : IDataConvertableFrom<SerializedChallenge, GameChallenge>
{
    [XmlElement("id")] public int ChallengeId { get; set; }
    [XmlElement("name")] public string Name { get; set; } = string.Empty;
    [XmlElement("slot")] public SerializedPhotoLevel Level { get; set; }
    [XmlElement("author")] public string AuthorName { get; set; } = string.Empty;
    [XmlElement("score")] public long Score { get; set; }
    [XmlElement("start-checkpoint")] public int StartCheckpointUid { get; set; }
    [XmlElement("end-checkpoint")] public int EndCheckpointUid { get; set; }
    [XmlElement("published")] public long Published { get; set; }  // Time in days
    [XmlElement("expires")] public long Expires { get; set; }  // Time in days
    [XmlArray("criteria")] public List<SerializedChallengeCriterion> Criteria { get; set; } = [];


    public static SerializedChallenge? FromOld(GameChallenge? old, DataContext dataContext)
    {
        if (old == null)
            return null;

        return new SerializedChallenge
        {
            ChallengeId = old.ChallengeId,
            Name = old.Name,
            Level = new SerializedPhotoLevel
            {
                LevelId = old.Level.LevelId,
                Title = old.Level.Title,
                Type = old.Level.LevelType.ToGameString(),
            },
            AuthorName = old.Publisher.Username,
            Score = dataContext.Database.GetOriginalChallengeScoreForChallenge(old).Score,
            StartCheckpointUid = old.StartCheckpointId,
            EndCheckpointUid = old.EndCheckpointId,
            Published = ToDays(old.PublishDate),
            Expires = ToDays(old.ExpirationDate),
            Criteria = SerializedChallengeCriterion.FromOldList(dataContext.Database.GetChallengeCriteria(old), dataContext).ToList(),
        };
    }

    public static IEnumerable<SerializedChallenge> FromOldList(IEnumerable<GameChallenge> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);

    public static long ToDays(DateTimeOffset dateTimeOffset)
        => dateTimeOffset.ToUnixTimeSeconds() / 60 / 60 / 24;  // DateTimeOffset -> Seconds -> Minutes -> Hours -> Days
    
    public static DateTimeOffset ToDateTimeOffset(long days)
        => DateTimeOffset.FromUnixTimeSeconds(days * 24 * 60 * 60);  // Days -> Hours -> Minutes -> Seconds -> DateTimeOffset
}