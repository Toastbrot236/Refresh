namespace Refresh.Database.Models.Photos;

public enum PhotoUploadSource : byte 
{
    /// <summary>
    /// A regular photo as you'd normally expect it, 
    /// taken by the game's snapshot tool as a sticker (using the popit or the start menu),
    /// and then uploaded.
    /// </summary>
    SnapshotTool,

    /// <summary>
    /// The screenshot included in a grief report, which was requested to be converted 
    /// into a GamePhoto instead.
    /// </summary>
    GriefToPhoto,
}