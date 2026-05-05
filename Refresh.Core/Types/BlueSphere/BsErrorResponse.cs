namespace Refresh.Core.Types.BlueSphere;

/// <summary>
/// Referenced from https://bluesphere.live/verify
/// </summary>
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class BsErrorResponse 
{
    /// <summary>
    /// Error message
    /// </summary>
    public required string Error { get; set; }
}