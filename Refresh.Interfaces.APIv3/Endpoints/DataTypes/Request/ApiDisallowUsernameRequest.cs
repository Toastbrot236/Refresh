namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiDisallowUsernameRequest
{
    public string Username { get; set; } = "";
    public string? Reason { get; set; }
}