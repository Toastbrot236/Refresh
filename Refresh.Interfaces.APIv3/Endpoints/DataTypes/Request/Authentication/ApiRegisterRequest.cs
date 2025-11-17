namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request.Authentication;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public abstract class ApiRegisterRequest
{
    public string EmailAddress { get; set; } = "";
    public string PasswordSha512 { get; set; } = "";
}