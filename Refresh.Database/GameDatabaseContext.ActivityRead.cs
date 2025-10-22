using System.Diagnostics;
using Refresh.Database.Models.Activity;
using Refresh.Database.Models.Users;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Photos;
using Refresh.Database.Models.Relations;
namespace Refresh.Database;

public partial class GameDatabaseContext // ActivityRead
{
    public GameUser? GetUserFromEvent(Event e)
    {
        if (e.StoredDataType != EventDataType.User)
            throw new InvalidOperationException($"Event does not store the correct data type (expected {nameof(EventDataType.User)})");

        Debug.Assert(e.StoredObjectId != null);

        return this.GetUserByObjectId(e.StoredObjectId);
    }

    public List<GameUser> GetUsersFromEvents(IReadOnlyCollection<Event> events)
    {
        // First add actors
        List<GameUser> users = events.Select(e => e.User).ToList();
        
        // Now add all objects which still exist (not deleted) aswell as referenced involved users
        foreach (Event e in events)
        {
            if (e.InvolvedUser != null) users.Add(e.InvolvedUser);
            
            // If this isn't even a user event, skip the object
            if (e.StoredDataType != EventDataType.User) continue;

            GameUser? obj = this.GetUserFromEvent(e);
            if (obj != null) users.Add(obj);
        }

        return users.DistinctBy(u => u.UserId).ToList();
    }

    public GameLevel? GetLevelFromEvent(Event e)
    {
        if (e.StoredDataType != EventDataType.Level)
            throw new InvalidOperationException($"Event does not store the correct data type (expected {nameof(EventDataType.Level)})");

        Debug.Assert(e.StoredSequentialId != null);
        
        return this.GetLevelById(e.StoredSequentialId.Value);
    }

    public List<GameLevel> GetLevelsFromEvents(IReadOnlyCollection<Event> events)
    {
        List<GameLevel> levels = [];

        // Add all objects which still exist (not deleted)
        foreach (Event e in events.Where(e => e.StoredDataType == EventDataType.Level))
        {
            GameLevel? obj = this.GetLevelFromEvent(e);
            if (obj != null) levels.Add(obj);
        }

        return levels.DistinctBy(l => l.LevelId).ToList();
    }

    public GameScore? GetScoreFromEvent(Event e)
    {
        if (e.StoredDataType != EventDataType.Score)
            throw new InvalidOperationException($"Event does not store the correct data type (expected {nameof(EventDataType.Score)})");

        Debug.Assert(e.StoredObjectId != null);

        return this.GetScoreByObjectId(e.StoredObjectId);
    }

    public List<GameScore> GetScoresFromEvents(IReadOnlyCollection<Event> events)
    {
        List<GameScore> scores = [];

        // Add all objects which still exist (not deleted)
        foreach (Event e in events.Where(e => e.StoredDataType == EventDataType.Score))
        {
            GameScore? obj = this.GetScoreFromEvent(e);
            if (obj != null) scores.Add(obj);
        }

        return scores.DistinctBy(s => s.ScoreId).ToList();
    }

    public GamePhoto? GetPhotoFromEvent(Event e)
    {
        if (e.StoredDataType != EventDataType.Photo)
            throw new InvalidOperationException($"Event does not store the correct data type (expected {nameof(EventDataType.Photo)})");

        Debug.Assert(e.StoredSequentialId != null);

        return this.GetPhotoById(e.StoredSequentialId.Value);
    }

    public List<GamePhoto> GetPhotosFromEvents(IReadOnlyCollection<Event> events)
    {
        List<GamePhoto> photos = [];

        // Add all objects which still exist (not deleted)
        foreach (Event e in events.Where(e => e.StoredDataType == EventDataType.Photo))
        {
            GamePhoto? obj = this.GetPhotoFromEvent(e);
            if (obj != null) photos.Add(obj);
        }

        return photos.DistinctBy(p => p.PhotoId).ToList();
    }
    
    public RateLevelRelation? GetRateLevelRelationFromEvent(Event e)
    {
        if (e.StoredDataType != EventDataType.RateLevelRelation)
            throw new InvalidOperationException($"Event does not store the correct data type (expected {nameof(EventDataType.RateLevelRelation)})");

        Debug.Assert(e.StoredObjectId != null);

        return this.RateLevelRelations
            .FirstOrDefault(l => l.RateLevelRelationId == e.StoredObjectId);
    }

    public List<RateLevelRelation> GetRateLevelRelationsFromEvents(IReadOnlyCollection<Event> events)
    {
        List<RateLevelRelation> relations = [];

        // Add all objects which still exist (not deleted)
        foreach (Event e in events.Where(e => e.StoredDataType == EventDataType.RateLevelRelation))
        {
            RateLevelRelation? obj = this.GetRateLevelRelationFromEvent(e);
            if (obj != null) relations.Add(obj);
        }

        return relations.DistinctBy(r => r.RateLevelRelationId).ToList();
    }
}