using System.Xml.Serialization;
using Refresh.Common.Constants;
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
    [XmlElement("author")] public string PublisherName { get; set; } = SystemUsers.UnknownUserName;
    [XmlElement("score")] public long Score { get; set; }
    [XmlElement("start-checkpoint")] public int StartCheckpointUid { get; set; }
    [XmlElement("end-checkpoint")] public int EndCheckpointUid { get; set; }
    [XmlElement("published")] public long Published { get; set; }  // Time in days, now - publish date
    [XmlElement("expires")] public long Expires { get; set; }  // Time in days, now - expiration date
    [XmlArray("criteria")] public List<SerializedChallengeCriterion> Criteria { get; set; } = [];

    public static SerializedChallenge? FromOld(GameChallenge? old, DataContext dataContext)
    {
        if (old == null)
            return null;
        
        long nowInMilliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        return new SerializedChallenge
        {
            ChallengeId = old.ChallengeId,
            Name = old.Name,
            Level = new SerializedPhotoLevel
            {
                LevelId = old.Level.LevelId,
                Type = old.Level.LevelType.ToGameString(),
                Title = "",  // does nothing if filled out
            },
            PublisherName = old.Publisher.Username,
            Score = dataContext.Database.GetOriginalScoreForChallenge(old)?.Score ?? 0,
            StartCheckpointUid = old.StartCheckpointUid,
            EndCheckpointUid = old.EndCheckpointUid,
            Published = ToDays(old.CreationDate.ToUnixTimeMilliseconds() - nowInMilliseconds),
            Expires = ToDays(old.ExpirationDate.ToUnixTimeMilliseconds() - nowInMilliseconds),
            Criteria = SerializedChallengeCriterion.FromOldList(dataContext.Database.GetChallengeCriteria(old), dataContext).ToList(),
        };
    }

    public static IEnumerable<SerializedChallenge> FromOldList(IEnumerable<GameChallenge> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);

    public static long ToDays(long milliseconds)
        => milliseconds / 1000 / 60 / 60 / 24;  // Milliseconds -> Seconds -> Minutes -> Hours -> Days
    
    public static long ToUnixMilliseconds(long days)
        => days * 24 * 60 * 60 * 1000;  // Days -> Hours -> Minutes -> Seconds -> Milliseconds
}