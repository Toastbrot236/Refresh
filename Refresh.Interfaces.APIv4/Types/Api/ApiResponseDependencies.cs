using System.Net;
using Bunkum.Core.Responses;

namespace Refresh.Interfaces.APIv4.Types.Api;

/// <summary>
/// Allows recent activity pages, aswell as various listings to deduplicate the level and user data they contain,
/// to try and make the response size smaller incase the same exact user or level appears multiple times,
/// while also making it easier for clients/encouraging them to implement caching.
/// <br/><br/>
/// The lists here will only contain the levels/users which are actually referenced in the current response.
/// </summary>
public class ApiResponseDependencies
{
    public ApiResponseDependencies()
    {
        
    }
    
    public List<int> Levels { get; set; } = [];
    public List<int> Users { get; set; } = [];
    public List<int> ExtendedUsers { get; set; } = [];
}