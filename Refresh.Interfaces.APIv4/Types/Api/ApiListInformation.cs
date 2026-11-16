
namespace Refresh.Interfaces.APIv4.Types.Api;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiListInformation
{
    /// <summary>
    /// Incase a client requests a page in the middle, this will allow it to also paginate backwards.
    /// </summary>
    public int PreviousPageIndex { get; set; }
    public int NextPageIndex { get; set; }
    public int TotalItems { get; set; }
}