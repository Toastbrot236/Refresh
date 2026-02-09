namespace Refresh.Database.Query;

public interface IPhotoUploadRequest
{
    public long Timestamp { get; set; }
    public string AuthorName { get; set; }
    
    public string SmallHash { get; set; }
    public string MediumHash { get; set; }
    public string LargeHash { get; set; }
    public string PlanHash { get; set; }

    public int LevelId { get; set; }
    public string LevelTitle { get; set; }
    public string LevelType { get; set; }
}