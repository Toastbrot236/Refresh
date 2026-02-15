using Refresh.Core.Types.Data;
using Refresh.Database.Models.Assets;

namespace Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Data;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiDisallowedAssetResponse : IApiResponse, IDataConvertableFrom<ApiDisallowedAssetResponse, DisallowedAsset>
{
    public required string AssetHash { get; set; }
    public required string Reason { get; set; }
    public required DateTimeOffset BlockedAt { get; set; }
    
    public static ApiDisallowedAssetResponse? FromOld(DisallowedAsset? old, DataContext dataContext)
    {
        if (old == null) return null;

        return new()
        {
            AssetHash = old.AssetHash,
            Reason = old.Reason,
            BlockedAt = old.BlockedAt,
        };
    }

    public static IEnumerable<ApiDisallowedAssetResponse> FromOldList(IEnumerable<DisallowedAsset> oldList, DataContext dataContext) 
        => oldList.Select(old => FromOld(old, dataContext)).ToList()!;
}