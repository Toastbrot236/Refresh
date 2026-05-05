namespace Refresh.Core.Types.BlueSphere;

/// <summary>
/// Referenced from https://bluesphere.live/verify
/// </summary>
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class BsOAuthRequest
{
    public required string AuthCode { get; set; }
}