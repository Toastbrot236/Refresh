namespace Refresh.Database.Query;

/// <summary>
/// Parameters which are only used for search queries and therefore should not be in LevelFilterSettings
/// </summary>
public struct SearchQueryParameters
{
    public bool ExcludeTitle { get; set; } = false;
    public bool ExcludeDescription { get; set; } = false;
    public string Query { get; set; } = "";
    public string DbQuery => $"%{this.Query}%";

    public SearchQueryParameters()
    {}
}