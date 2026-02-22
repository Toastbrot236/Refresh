namespace Refresh.Database.Query;

public class SearchSettings
{
    /// <summary>
    /// Whether to look for the search query in the level titles/usernames/playlist names
    /// </summary>
    public bool CompareNames { get; set; }

    /// <summary>
    /// Whether to look for the search query in the level/user/playlist descriptions
    /// </summary>
    public bool CompareDescriptions { get; set; }
}