namespace Refresh.Database.Query;

public interface IPhotoUploadSubject
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public float[] BoundsParsed { get; set; }
}