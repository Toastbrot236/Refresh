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

    /// <summary>
    /// Probably the score you are supposed to beat. Always 0 when challenge is first uploaded by LBP hub.
    /// </summary>
    [XmlElement("score")] public long Score { get; set; }

    [XmlElement("start-checkpoint")] public int StartCheckpointUid { get; set; }

    /// <summary>
    /// The Uid of the checkpoint this challenge finishes on.
    /// </summary>
    [XmlElement("end-checkpoint")] public int EndCheckpointUid { get; set; }

    /// <summary>
    /// Sent by the game as time in days, which is normally always 0 here.
    /// But for the response we have to send the actual milliseconds of the creation date, else lbp hub will crash.
    /// </summary>
    [XmlElement("published")] public long Published { get; set; }

    /// <summary>
    /// Sent by the game as time in days, which is usually 3, 5 or 7 here, as those are the only selectable options ingame.
    /// But for the response we have to send the actual milliseconds of the expiration date, else lbp hub will crash.
    /// </summary>
    [XmlElement("expires")] public long Expires { get; set; }

    /// <summary>
    /// An array of criteria of a challenge, sent and expected by LBP hub. Appears to only ever have a single criterion.
    /// </summary>
    /// <seealso cref="SerializedChallengeCriterion"/>
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
                LevelId = old.Level.SlotType == GameSlotType.Story ? old.Level.StoryId : old.Level.LevelId,
                Type = old.Level.SlotType.ToGameType(),
                Title = old.Level.Title,  // does nothing if filled out
            },
            PublisherName = old.Publisher.Username,
            Score = 0,
            StartCheckpointUid = old.StartCheckpointUid,
            EndCheckpointUid = old.EndCheckpointUid,
            Published = old.PublishDate.ToUnixTimeMilliseconds(),
            Expires = old.ExpirationDate.ToUnixTimeMilliseconds(),
            Criteria =
            [
                new()
                {
                    Type = (byte)old.Type,
                    Value = 0,
                }
            ],
        };
    }

    public static IEnumerable<SerializedChallenge> FromOldList(IEnumerable<GameChallenge> oldList, DataContext dataContext)
        => oldList.Select(c => FromOld(c, dataContext)!);

    public static long ToDays(long milliseconds)
        => milliseconds / 1000 / 60 / 60 / 24;  // Milliseconds -> Seconds -> Minutes -> Hours -> Days
    
    public static long ToUnixMilliseconds(long days)
        => days * 24 * 60 * 60 * 1000;  // Days -> Hours -> Minutes -> Seconds -> Milliseconds
}