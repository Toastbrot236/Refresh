using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Assets;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Documentation.Attributes;
using Refresh.Interfaces.APIv3.Documentation.Descriptions;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Data;
using Refresh.Interfaces.APIv3.Extensions;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

public class AdminAssetApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("admin/users/{idType}/{id}/assets"), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Retrieves a list of assets uploaded by the user.")]
    [DocQueryParam("assetType", "The asset type to filter by. Can be excluded to list all types.")]
    [DocUsesPageData]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), "The asset type could not be parsed.")]
    public ApiListResponse<ApiMinimalGameAssetResponse> GetAssetsByUser(RequestContext context, GameDatabaseContext database, DataContext dataContext,
        [DocSummary(SharedParamDescriptions.UserIdParam)] string id, 
        [DocSummary(SharedParamDescriptions.UserIdTypeParam)] string idType)
    {
        GameUser? user = database.GetUserByIdAndType(idType, id);
        if (user == null) return ApiNotFoundError.UserMissingError;
        
        (int skip, int count) = context.GetPageData(1000);

        DatabaseList<GameAsset> assets;

        string? assetTypeStr = context.QueryString.Get("assetType");
        if (assetTypeStr == null)
        {
            assets = database.GetAssetsUploadedByUser(user, skip, count);
        }
        else
        {
            bool parsed = Enum.TryParse(assetTypeStr, true, out GameAssetType assetType);
            if (!parsed)
                return new ApiValidationError($"The asset type '{assetTypeStr}' couldn't be parsed. Possible values: "
                    + string.Join(", ", Enum.GetNames(typeof(GameAssetType))));
            
            assets = database.GetAssetsUploadedByUser(user, skip, count, assetType);
        }
        
        return DatabaseListExtensions.FromOldList<ApiMinimalGameAssetResponse, GameAsset>(assets, dataContext);
    }

    #region Disallowed Assets

    [ApiV3Endpoint("admin/disallowed/assets", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Disallows a specific asset.")]
    public ApiResponse<ApiDisallowedAssetResponse> DisallowAsset(RequestContext context, DataContext dataContext, GameUser user, ApiDisallowAssetRequest body)
    {
        GameAssetType? type;
        if (body.AssetType == null)
        {
            type = dataContext.Database.GetAssetFromHash(body.AssetHash)?.AssetType;
        }
        else
        {
            bool parsed = Enum.TryParse(body.AssetType, true, out GameAssetType assetType);
            if (!parsed) {
                return new ApiValidationError($"The asset type '{body.AssetType}' couldn't be parsed. Possible values: "
                    + string.Join(", ", Enum.GetNames(typeof(GameAssetType))));
            }

            type = assetType;
        }

        (DisallowedAsset info, bool success) = dataContext.Database.DisallowAsset(body.AssetHash, type ?? GameAssetType.Unknown, body.Reason ?? "");
        // TODO: mod log
        return new ApiResponse<ApiDisallowedAssetResponse>(ApiDisallowedAssetResponse.FromOld(info, dataContext)!, success ? Created : OK);
    }

    [ApiV3Endpoint("admin/disallowed/assets", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Reallows a specific asset.")]
    public ApiOkResponse ReallowAsset(RequestContext context, DataContext dataContext, GameUser user, ApiDisallowAssetRequest body)
    {
        bool success = dataContext.Database.ReallowAsset(body.AssetHash);
        // TODO: mod log
        if (!success) return ApiNotFoundError.Instance;
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("admin/disallowed/assets", HttpMethods.Get), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Gets all disallowed assets.")]
    [DocQueryParam("assetType", "The asset type to filter by. Can be excluded to list all types.")]
    [DocUsesPageData]
    public ApiListResponse<ApiDisallowedAssetResponse> GetDisallowedAssets(RequestContext context, DataContext dataContext, GameUser user)
    {
        (int skip, int count) = context.GetPageData();
        string? assetTypeStr = context.QueryString.Get("assetType");

        DatabaseList<DisallowedAsset> disallowedList;

        if (assetTypeStr == null)
        {
            disallowedList = dataContext.Database.GetDisallowedAssets(skip, count);
        }
        else
        {
            bool parsed = Enum.TryParse(assetTypeStr, true, out GameAssetType assetType);
            if (!parsed)
                return new ApiValidationError($"The asset type '{assetTypeStr}' couldn't be parsed. Possible values: "
                    + string.Join(", ", Enum.GetNames(typeof(GameAssetType))));
            
            disallowedList = dataContext.Database.GetDisallowedAssetsByType(assetType, skip, count);
        }

        return DatabaseListExtensions.FromOldList<ApiDisallowedAssetResponse, DisallowedAsset>(disallowedList, dataContext);
    }

    #endregion
}