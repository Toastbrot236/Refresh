using Refresh.Database.Models.Activity;
using Refresh.Database.Models.Users;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Photos;
using Refresh.Database.Models.Relations;
using Refresh.Database.Query;
using Refresh.Database.Models.Comments;
using Refresh.Database.Models.Playlists;
using Refresh.Database.Models.Levels.Challenges;
namespace Refresh.Database;

public partial class GameDatabaseContext // ActivityWrite
{
    public Event CreateEvent(GameUser user, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.User,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = user,
            StoredObjectId = user.UserId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });
    
    public Event CreateEvent(GameLevel level, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.Level,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = level.Publisher,
            StoredSequentialId = level.LevelId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });
    
    public Event CreateEvent(GameScore score, EventCreationParams param, GameUser scoreUploader) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.Score,
            Timestamp = this._time.Now,
            User = param.Actor,
            // TODO: store the foreign key of the actual score uploader seperately from the player list, and use that here
            ObjectPublisher = scoreUploader,
            StoredObjectId = score.ScoreId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });
    
    public Event CreateEvent(RateLevelRelation relation, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.RateLevelRelation,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = relation.User,
            StoredObjectId = relation.RateLevelRelationId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });
    
    public Event CreateEvent(GamePhoto photo, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.Photo,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = photo.Publisher,
            StoredSequentialId = photo.PhotoId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });
    
    public Event CreateEvent(GameReview review, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.Review,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = review.Publisher,
            StoredSequentialId = review.ReviewId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });
    
    public Event CreateEvent(GameProfileComment comment, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.UserComment,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = comment.Author,
            StoredSequentialId = comment.SequentialId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });

    public Event CreateEvent(GameLevelComment comment, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.LevelComment,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = comment.Author,
            StoredSequentialId = comment.SequentialId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });

    public Event CreateEvent(GamePlaylist playlist, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.Playlist,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = playlist.Publisher,
            StoredSequentialId = playlist.PlaylistId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });

    public Event CreateEvent(GameChallenge challenge, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.Challenge,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = challenge.Publisher,
            StoredSequentialId = challenge.ChallengeId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });

    public Event CreateEvent(GameChallengeScore score, EventCreationParams param) => 
        this.CreateEvent(new()
        {
            EventType = param.EventType,
            StoredDataType = EventDataType.ChallengeScore,
            Timestamp = this._time.Now,
            User = param.Actor,
            ObjectPublisher = score.Publisher,
            StoredObjectId = score.ScoreId,
            IsModified = param.IsModified,
            IsPrivate = param.IsPrivate,
        });
    
    // TODO: Event creation methods for Contest, Asset and PinProgress once the ID storing is figured out for them

    private Event CreateEvent(Event e)
    {
        this.Events.Add(e);
        this.SaveChanges();
        return e;
    }
}