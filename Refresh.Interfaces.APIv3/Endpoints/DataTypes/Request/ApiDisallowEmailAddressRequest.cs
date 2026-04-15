namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiDisallowEmailAddressRequest
{
    public string Address { get; set; } = "";
    public string? Reason { get; set; }
}