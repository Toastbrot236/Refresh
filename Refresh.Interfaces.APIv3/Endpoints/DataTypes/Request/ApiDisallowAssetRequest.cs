using Refresh.Database.Models.Assets;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiDisallowAssetRequest
{
    public string? Reason { get; set; }
    public GameAssetType? AssetType { get; set; }
}