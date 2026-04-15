namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiDisallowEmailDomainRequest
{
    public string Domain { get; set; } = "";
    public string? Reason { get; set; }
}