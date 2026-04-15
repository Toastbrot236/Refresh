namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiDisallowAssetRequest
{
    public string AssetHash { get; set; } = "";
    /// <summary>
    /// String or numeric enum value of <see cref='Database.Models.Assets.GameAssetType'/>
    /// </summary>
    public string? AssetType { get; set; }
    public string? Reason { get; set; }
}