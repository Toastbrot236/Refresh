namespace Refresh.GameServer.Endpoints.ApiV3.DataTypes.Request;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGameLocationRequest
{
    public required int X { get; set; }
    public required int Y { get; set; }
}