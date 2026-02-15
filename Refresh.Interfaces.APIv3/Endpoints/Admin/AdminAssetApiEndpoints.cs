using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Protocols.Http;
using Refresh.Common.Verification;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Assets;
using Refresh.Database.Models.Moderation;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Data;
using Refresh.Interfaces.APIv3.Extensions;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

public class AdminAssetApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("admin/users/uuid/{uuid}/assets"), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Retrieves a list of assets uploaded by the user.")]
    [DocQueryParam("assetType", "The asset type to filter by. Can be excluded to list all types.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), "The asset type could not be parsed.")]
    public ApiListResponse<ApiMinimalGameAssetResponse> GetAssetsByUser(RequestContext context, GameDatabaseContext database, DataContext dataContext,
        [DocSummary("The UUID of the user.")] string uuid)
    {
        GameUser? user = database.GetUserByUuid(uuid);
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

    [ApiV3Endpoint("admin/assets/blocked"), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Retrieves a list of blocked asset hashes, together with reasons on why they were blocked.")]
    public ApiListResponse<ApiDisallowedAssetResponse> GetDisallowedAssets(RequestContext context, GameDatabaseContext database, DataContext dataContext)
    {
        (int skip, int count) = context.GetPageData(1000);

        DatabaseList<DisallowedAsset> disallowedAssets = database.GetDisallowedAssets(skip, count);
        return DatabaseListExtensions.FromOldList<ApiDisallowedAssetResponse, DisallowedAsset>(disallowedAssets, dataContext);
    }

    [ApiV3Endpoint("admin/assets/blocked/{assetHash}"), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Gets information about a blocked asset if it's blocked.")]
    public ApiResponse<ApiDisallowedAssetResponse> GetDisallowedAsset(RequestContext context, GameDatabaseContext database, DataContext dataContext, 
        [DocSummary("The hash of the blocked asset.")] string assetHash, GameUser user)
    {
        // Should there be an extra parameter to bypass hash validation?
        if (!CommonPatterns.Sha1Regex().IsMatch(assetHash)) 
            return ApiValidationError.HashInvalidError;
        
        DisallowedAsset? disallowedAsset = database.GetDisallowedAssetByHash(assetHash);
        if (disallowedAsset == null) 
            return ApiNotFoundError.Instance; // TODO: message

        return ApiDisallowedAssetResponse.FromOld(disallowedAsset, dataContext);
    }

    [ApiV3Endpoint("admin/assets/blocked/{assetHash}", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Blocks an asset by hash, ignoring whether it is uploaded to the server or not.")]
    public ApiResponse<ApiDisallowedAssetResponse> DisallowAsset(RequestContext context, GameDatabaseContext database, DataContext dataContext, 
        [DocSummary("The asset hash to be blocked.")] string assetHash, GameUser user,
        [DocSummary("Contains the reason, and might potentially also contain other data in the future.")] ApiDisallowAssetRequest body)
    {
        // Should there be an extra parameter to bypass hash validation?
        if (!CommonPatterns.Sha1Regex().IsMatch(assetHash)) 
            return ApiValidationError.HashInvalidError;
        
        DisallowedAsset? disallowedAsset = database.GetDisallowedAssetByHash(assetHash);
        if (disallowedAsset != null) 
            return ApiDisallowedAssetResponse.FromOld(disallowedAsset, dataContext);
        
        disallowedAsset = database.DisallowAsset(assetHash, body.Reason);

        // Is the blocked asset already on the server?
        GameAsset? asset = database.GetAssetFromHash(assetHash);
        if (asset != null)
        {
            database.CreateModerationAction(asset, ModerationActionType.BlockAsset, user, body.Reason);
        }
        else
        {
            database.CreateModerationAction(assetHash, ModerationActionType.BlockAsset, user, body.Reason);
        }

        // TODO: Reset icons of whatever was using this asset as icon, and delete anything else that was referencing this asset in any other way
        // (excluding reports)?

        return new ApiResponse<ApiDisallowedAssetResponse>(ApiDisallowedAssetResponse.FromOld(disallowedAsset, dataContext)!, Created);
    }

    [ApiV3Endpoint("admin/assets/blocked/{assetHash}", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Unblocks the asset specified by hash, ignoring whether it is uploaded to the server or not.")]
    public ApiOkResponse ReallowAsset(RequestContext context, GameDatabaseContext database, DataContext dataContext, 
        [DocSummary("The asset hash to be unblocked.")] string assetHash, GameUser user)
    {
        // Should there be an extra parameter to bypass hash validation?
        if (!CommonPatterns.Sha1Regex().IsMatch(assetHash)) 
            return ApiValidationError.HashInvalidError;
        
        DisallowedAsset? disallowedAsset = database.GetDisallowedAssetByHash(assetHash);
        if (disallowedAsset == null)
            return ApiNotFoundError.Instance; // TODO: message
        
        database.ReallowAsset(assetHash);

        // Is the unblocked asset already on the server?
        GameAsset? asset = database.GetAssetFromHash(assetHash);
        if (asset != null)
        {
            database.CreateModerationAction(asset, ModerationActionType.UnblockAsset, user, "");
        }
        else
        {
            database.CreateModerationAction(assetHash, ModerationActionType.UnblockAsset, user, "");
        }

        return new ApiOkResponse();
    }
}