namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request.Authentication;

#nullable disable

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiRegisterByCodeRequest : ApiRegisterRequest
{
    public string Code { get; set; }
}