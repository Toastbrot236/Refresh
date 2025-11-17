namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request.Authentication;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiRegisterByNameRequest : ApiRegisterRequest
{
    public string Username { get; set; } = "";
}