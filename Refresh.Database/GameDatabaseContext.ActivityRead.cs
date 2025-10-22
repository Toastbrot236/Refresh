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
        
        // Now add all objects which still exist (not deleted) and referenced involved users, 
        // while making sure there won't be any duplicates in the resulting list.
        foreach (Event e in events)
        {
            // If the involved user is not null and not already in the list, 
            // add them
            if (e.InvolvedUser != null && !users.Any(u => u.UserId == e.InvolvedUserId)) 
                users.Add(e.InvolvedUser);

            // If the object user is not in the list and hasn't been deleted off
            // the server yet, add them aswell
            GameUser? obj;
            if (!users.Any(u => u.UserId == e.StoredObjectId) && (obj = this.GetUserFromEvent(e)) != null)
                users.Add(obj);
        }

        return users;
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
        foreach (Event e in events)
        {
            GameLevel? obj = this.GetLevelFromEvent(e);
            if (obj != null) levels.Add(obj);
        }

        return levels;
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
        foreach (Event e in events)
        {
            GameScore? obj = this.GetScoreFromEvent(e);
            if (obj != null) scores.Add(obj);
        }

        return scores;
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
        foreach (Event e in events)
        {
            GamePhoto? obj = this.GetPhotoFromEvent(e);
            if (obj != null) photos.Add(obj);
        }

        return photos;
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
        foreach (Event e in events)
        {
            RateLevelRelation? obj = this.GetRateLevelRelationFromEvent(e);
            if (obj != null) relations.Add(obj);
        }

        return relations;
    }
}