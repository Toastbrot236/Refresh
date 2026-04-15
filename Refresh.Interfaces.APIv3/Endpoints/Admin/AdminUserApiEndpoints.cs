using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.Storage;
using Bunkum.Protocols.Http;
using Refresh.Common.Constants;
using Refresh.Common.Verification;
using Refresh.Core.Authentication.Permission;
using Refresh.Core.Types.Data;
using Refresh.Database;
using Refresh.Database.Models.Moderation;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Documentation.Attributes;
using Refresh.Interfaces.APIv3.Documentation.Descriptions;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Admin;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;
using Refresh.Interfaces.APIv3.Extensions;

namespace Refresh.Interfaces.APIv3.Endpoints.Admin;

using BC = BCrypt.Net.BCrypt;

public class AdminUserApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("admin/users/{idType}/{id}"), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Gets a user by their UUID or name with extended information.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiResponse<ApiExtendedGameUserResponse> GetExtendedUser(RequestContext context,
        GameDatabaseContext database, DataContext dataContext,
        [DocSummary(SharedParamDescriptions.UserIdParam)] string id, 
        [DocSummary(SharedParamDescriptions.UserIdTypeParam)] string idType)
    {
        GameUser? user = database.GetUserByIdAndType(idType, id);
        if (user == null) return ApiNotFoundError.UserMissingError;

        return ApiExtendedGameUserResponse.FromOld(user, dataContext);
    }

    [ApiV3Endpoint("admin/users"), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Gets all users with extended information.")]
    [DocUsesPageData]
    public ApiListResponse<ApiExtendedGameUserResponse> GetExtendedUsers(RequestContext context,
        GameDatabaseContext database, IDataStore dataStore, DataContext dataContext)
    {
        (int skip, int count) = context.GetPageData();
        DatabaseList<ApiExtendedGameUserResponse> list = DatabaseListExtensions.FromOldList<ApiExtendedGameUserResponse, GameUser>(database.GetUsers(count, skip), dataContext);
        return list;
    }

    [ApiV3Endpoint("admin/users/{idType}/{id}/resetPassword", HttpMethods.Put), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Resets a user's password by their UUID or username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.MayNotModifyUserDueToLowRoleErrorWhen)]
    [DocRequestBody(typeof(ApiResetUserPasswordRequest))]
    public ApiOkResponse ResetUserPassword(RequestContext context, GameDatabaseContext database, ApiResetUserPasswordRequest body, GameUser user,
        [DocSummary(SharedParamDescriptions.UserIdParam)] string id, 
        [DocSummary(SharedParamDescriptions.UserIdTypeParam)] string idType)
    {
        GameUser? targetUser = database.GetUserByIdAndType(idType, id);
        if (targetUser == null) return ApiNotFoundError.UserMissingError;

        if (!user.MayModifyUser(targetUser))
            return ApiValidationError.MayNotModifyUserDueToLowRoleError;

        if (body.PasswordSha512.Length != 128 || !CommonPatterns.Sha512Regex().IsMatch(body.PasswordSha512))
            return new ApiValidationError("Password is definitely not SHA512. Please hash the password.");
        
        string? passwordBcrypt = BC.HashPassword(body.PasswordSha512, AuthenticationApiEndpoints.WorkFactor);
        if (passwordBcrypt == null) return new ApiInternalError("Could not BCrypt the given password.");
        
        database.SetUserPassword(targetUser, passwordBcrypt, true);
        return new ApiOkResponse();
    }
    
    // TODO: Users should be able to retrieve and reset their own planets
    [ApiV3Endpoint("admin/users/{idType}/{id}/planets"), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Retrieves the hashes of a user's planets and whether they're modded. Gets user by their UUID or username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    public ApiResponse<ApiAdminUserPlanetsResponse> GetUserPlanets(RequestContext context, GameDatabaseContext database,
        [DocSummary(SharedParamDescriptions.UserIdParam)] string id, 
        [DocSummary(SharedParamDescriptions.UserIdTypeParam)] string idType)
    {
        GameUser? user = database.GetUserByIdAndType(idType, id);
        if (user == null) return ApiNotFoundError.UserMissingError;

        return new ApiAdminUserPlanetsResponse
        {
            Lbp2PlanetsHash = user.Lbp2PlanetsHash,
            Lbp3PlanetsHash = user.Lbp3PlanetsHash,
            VitaPlanetsHash = user.VitaPlanetsHash,
            BetaPlanetsHash = user.BetaPlanetsHash,
            AreLbp2PlanetsModded = user.AreLbp2PlanetsModded,
            AreLbp3PlanetsModded = user.AreLbp3PlanetsModded,
            AreVitaPlanetsModded = user.AreVitaPlanetsModded,
            AreBetaPlanetsModded = user.AreBetaPlanetsModded,
        };
    }
    
    [ApiV3Endpoint("admin/users/{idType}/{id}/planets", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Resets a user's planets. Gets user by their UUID or username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.MayNotModifyUserDueToLowRoleErrorWhen)]
    public ApiOkResponse ResetUserPlanets(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary(SharedParamDescriptions.UserIdParam)] string id, 
        [DocSummary(SharedParamDescriptions.UserIdTypeParam)] string idType)
    {
        GameUser? targetUser = database.GetUserByIdAndType(idType, id);
        if (targetUser == null) return ApiNotFoundError.UserMissingError;

        if (!user.MayModifyUser(targetUser))
            return ApiValidationError.MayNotModifyUserDueToLowRoleError;

        database.ResetUserPlanets(targetUser);
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("admin/users/{idType}/{id}", HttpMethods.Patch), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Updates the specified user's profile with the given data")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.MayNotModifyUserDueToLowRoleErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.MayNotOverwriteRoleErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.RoleMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.WrongRoleUpdateMethodErrorWhen)]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.IconMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.InvalidUsernameErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.UsernameTakenErrorWhen)]
    public ApiResponse<ApiExtendedGameUserResponse> UpdateUser(RequestContext context, GameDatabaseContext database,
        GameUser user, ApiAdminUpdateUserRequest body, DataContext dataContext,
        [DocSummary(SharedParamDescriptions.UserIdParam)] string id, 
        [DocSummary(SharedParamDescriptions.UserIdTypeParam)] string idType)
    {
        GameUser? targetUser = database.GetUserByIdAndType(idType, id);

        if (targetUser == null)
            return ApiNotFoundError.UserMissingError;

        if (!user.MayModifyUser(targetUser))
            return ApiValidationError.MayNotModifyUserDueToLowRoleError;

        // Only admins may edit anyone's role.
        // TODO: Maybe moderators should also be able to set roles, but only for users below them, and to roles below them?
        if (body.Role != null)
        {
            if (user.Role < GameUserRole.Admin)
                return ApiValidationError.MayNotOverwriteRoleError;

            if (!Enum.IsDefined(typeof(GameUserRole), body.Role))
                return ApiValidationError.RoleMissingError;

            // All roles below regular user are special and must be given using different endpoints because they require extra information.
            // Incase the implementation of #286 requires a guest role, that one will very likely be below User aswell, and it should also not
            // be assignable with this endpoint (when should a user ever be demoted to a temporary guest?)
            if (body.Role < GameUserRole.User)
                return ApiValidationError.WrongRoleUpdateMethodError;
        }

        (body.IconHash, ApiError? mainIconError) = body.IconHash.ValidateIcon(dataContext);
        if (mainIconError != null) return mainIconError;

        (body.VitaIconHash, ApiError? vitaIconError) = body.VitaIconHash.ValidateIcon(dataContext);
        if (vitaIconError != null) return vitaIconError;

        (body.BetaIconHash, ApiError? betaIconError) = body.BetaIconHash.ValidateIcon(dataContext);
        if (betaIconError != null) return betaIconError;

        // Do nothing if the username entered is actually the same as the one already set
        if (body.Username != null && body.Username != targetUser.Username) 
        {
            if (!body.Username.StartsWith(SystemUsers.SystemPrefix) && !database.IsUsernameValid(body.Username))
                return new ApiValidationError(ApiValidationError.InvalidUsernameErrorWhen
                    + " Are you sure you used a PSN/RPCN username, or prepended it with ! if it's a fake user?");
            
            if (database.IsUsernameTaken(body.Username, targetUser))
                return ApiValidationError.UsernameTakenError;
            
            database.RenameUser(targetUser, body.Username);
        }

        // Trim description
        if (body.Description != null && body.Description.Length > UgcLimits.DescriptionLimit)
            body.Description = body.Description[..UgcLimits.DescriptionLimit];

        database.UpdateUserData(targetUser, body);
        database.CreateModerationAction(targetUser, ModerationActionType.UserModification, user, "");

        return ApiExtendedGameUserResponse.FromOld(targetUser, dataContext);
    }
    
    [ApiV3Endpoint("admin/users/{idType}/{id}", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Deletes a user by their UUID or username.")]
    [DocError(typeof(ApiNotFoundError), ApiNotFoundError.UserMissingErrorWhen)]
    [DocError(typeof(ApiValidationError), ApiValidationError.MayNotModifyUserDueToLowRoleErrorWhen)]
    public ApiOkResponse DeleteUser(RequestContext context, GameDatabaseContext database, GameUser user,
        [DocSummary(SharedParamDescriptions.UserIdParam)] string id, 
        [DocSummary(SharedParamDescriptions.UserIdTypeParam)] string idType)
    {
        GameUser? targetUser = database.GetUserByIdAndType(idType, id);
        if (targetUser == null) return ApiNotFoundError.UserMissingError;

        if (!user.MayModifyUser(targetUser))
            return ApiValidationError.MayNotModifyUserDueToLowRoleError;

        database.DeleteUser(targetUser);
        return new ApiOkResponse();
    }

    #region Disallowed Email Addresses

    [ApiV3Endpoint("admin/disallowed/emailAddresses", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Disallows a specific email address.")]
    public ApiResponse<ApiDisallowedEmailAddressResponse> DisallowEmailAddress(RequestContext context, DataContext dataContext, GameUser user, ApiDisallowEmailAddressRequest body)
    {
        (DisallowedEmailAddress info, bool success) = dataContext.Database.DisallowEmailAddress(body.Address, body.Reason ?? "");
        // TODO: mod log
        return new ApiResponse<ApiDisallowedEmailAddressResponse>(ApiDisallowedEmailAddressResponse.FromOld(info, dataContext)!, success ? Created : OK);
    }

    [ApiV3Endpoint("admin/disallowed/emailAddresses", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Reallows a specific email address.")]
    public ApiOkResponse ReallowEmailAddress(RequestContext context, DataContext dataContext, GameUser user, ApiDisallowEmailAddressRequest body)
    {
        bool success = dataContext.Database.ReallowEmailAddress(body.Address);
        // TODO: mod log
        if (!success) return ApiNotFoundError.Instance;
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("admin/disallowed/emailAddresses", HttpMethods.Get), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Gets all disallowed email addresses.")]
    [DocUsesPageData]
    public ApiListResponse<ApiDisallowedEmailAddressResponse> GetDisallowedEmailAddresses(RequestContext context, DataContext dataContext, GameUser user)
    {
        (int skip, int count) = context.GetPageData();
        DatabaseList<DisallowedEmailAddress> disallowedList = dataContext.Database.GetDisallowedEmailAddresses(skip, count);
        return DatabaseListExtensions.FromOldList<ApiDisallowedEmailAddressResponse, DisallowedEmailAddress>(disallowedList, dataContext);
    }

    #endregion
    
    #region Disallowed Email Domains

    [ApiV3Endpoint("admin/disallowed/emailDomains", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Disallows a specific email domain. If the given domain is a whole email address, only the part after the @ will be used as the domain.")]
    public ApiResponse<ApiDisallowedEmailDomainResponse> DisallowEmailDomain(RequestContext context, DataContext dataContext, GameUser user, ApiDisallowEmailDomainRequest body)
    {
        (DisallowedEmailDomain info, bool success) = dataContext.Database.DisallowEmailDomain(body.Domain, body.Reason ?? "");
        // TODO: mod log
        return new ApiResponse<ApiDisallowedEmailDomainResponse>(ApiDisallowedEmailDomainResponse.FromOld(info, dataContext)!, success ? Created : OK);
    }

    [ApiV3Endpoint("admin/disallowed/emailDomains", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Reallows a specific email domain. If the given domain is a whole email address, only the part after the @ will be used as the domain.")]
    public ApiOkResponse ReallowEmailDomain(RequestContext context, DataContext dataContext, GameUser user, ApiDisallowEmailDomainRequest body)
    {
        bool success = dataContext.Database.ReallowEmailDomain(body.Domain);
        // TODO: mod log
        if (!success) return ApiNotFoundError.Instance;
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("admin/disallowed/emailDomains", HttpMethods.Get), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Gets a list of disallowed email domains.")]
    [DocUsesPageData]
    public ApiListResponse<ApiDisallowedEmailDomainResponse> GetDisallowedEmailDomains(RequestContext context, DataContext dataContext, GameUser user)
    {
        (int skip, int count) = context.GetPageData();
        DatabaseList<DisallowedEmailDomain> disallowedList = dataContext.Database.GetDisallowedEmailDomains(skip, count);
        return DatabaseListExtensions.FromOldList<ApiDisallowedEmailDomainResponse, DisallowedEmailDomain>(disallowedList, dataContext);
    }

    #endregion

    #region Disallowed Usernames

    [ApiV3Endpoint("admin/disallowed/usernames", HttpMethods.Post), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Disallows a specific username.")]
    public ApiResponse<ApiDisallowedUsernameResponse> DisallowUsername(RequestContext context, DataContext dataContext, GameUser user, ApiDisallowUsernameRequest body)
    {
        (DisallowedUser info, bool success) = dataContext.Database.DisallowUser(body.Username, body.Reason ?? "");
        // TODO: mod log
        return new ApiResponse<ApiDisallowedUsernameResponse>(ApiDisallowedUsernameResponse.FromOld(info, dataContext)!, success ? Created : OK);
    }

    [ApiV3Endpoint("admin/disallowed/usernames", HttpMethods.Delete), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Reallows a specific username.")]
    public ApiOkResponse ReallowUsername(RequestContext context, DataContext dataContext, GameUser user, ApiDisallowUsernameRequest body)
    {
        bool success = dataContext.Database.ReallowUser(body.Username);
        // TODO: mod log
        if (!success) return ApiNotFoundError.Instance;
        return new ApiOkResponse();
    }

    [ApiV3Endpoint("admin/disallowed/usernames", HttpMethods.Get), MinimumRole(GameUserRole.Moderator)]
    [DocSummary("Gets a list of disallowed usernames.")]
    [DocUsesPageData]
    public ApiListResponse<ApiDisallowedUsernameResponse> GetDisallowedUsernames(RequestContext context, DataContext dataContext, GameUser user)
    {
        (int skip, int count) = context.GetPageData();
        DatabaseList<DisallowedUser> disallowedList = dataContext.Database.GetDisallowedUsers(skip, count);
        return DatabaseListExtensions.FromOldList<ApiDisallowedUsernameResponse, DisallowedUser>(disallowedList, dataContext);
    }

    #endregion
}