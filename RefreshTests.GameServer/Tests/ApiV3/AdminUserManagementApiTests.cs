using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Users;
using RefreshTests.GameServer.Extensions;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request.Authentication;
using Refresh.Common.Helpers;
using System.Security.Cryptography;
using Refresh.Interfaces.Game.Types.UserData;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Admin;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Common.Extensions;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;

namespace RefreshTests.GameServer.Tests.ApiV3;

public class AdminUserManagementApiTests : GameServerTest
{
    [Test]
    public async Task ResetsUsersPasswordByUuidAndName()
    {
        using TestContext context = this.GetServer();
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);

        // UUID
        GameUser player1 = context.CreateUser(role: GameUserRole.User);
        ApiResetPasswordRequest request = new()
        {
            PasswordSha512 = HexHelper.BytesToHexString(SHA512.HashData("poo"u8))
        };
        HttpResponseMessage response = await client.PutAsync($"/api/v3/admin/users/uuid/{player1.UserId}/resetPassword", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.True);

        context.Database.Refresh();
        GameUser? updated1 = context.Database.GetUserByObjectId(player1.UserId);
        Assert.That(updated1, Is.Not.Null);
        Assert.That(updated1!.ShouldResetPassword, Is.True);

        // Name
        GameUser player2 = context.CreateUser(role: GameUserRole.User);
        request = new()
        {
            PasswordSha512 = HexHelper.BytesToHexString(SHA512.HashData("poo"u8))
        };
        response = await client.PutAsync($"/api/v3/admin/users/name/{player2.Username}/resetPassword", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.True);

        context.Database.Refresh();
        GameUser? updated2 = context.Database.GetUserByObjectId(player2.UserId);
        Assert.That(updated2, Is.Not.Null);
        Assert.That(updated2!.ShouldResetPassword, Is.True);
    }

    [Test]
    public async Task GetsAndResetsUserPlanetsByUuidAndName()
    {
        using TestContext context = this.GetServer();
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);
        GameUser player = context.CreateUser(role: GameUserRole.User);

        // UUID
        context.Database.UpdateUserData(player, new SerializedUpdateData()
        {
            PlanetsHash = "lol"
        }, TokenGame.LittleBigPlanet2);
        
        ApiResponse<ApiAdminUserPlanetsResponse>? planetResponse = client.GetData<ApiAdminUserPlanetsResponse>($"/api/v3/admin/users/uuid/{player.UserId}/planets");
        Assert.That(planetResponse?.Data, Is.Not.Null);
        Assert.That(planetResponse!.Data!.Lbp2PlanetsHash, Is.EqualTo("lol"));

        HttpResponseMessage resetResponse = await client.DeleteAsync($"/api/v3/admin/users/uuid/{player.UserId}/planets");
        Assert.That(resetResponse.IsSuccessStatusCode, Is.True);

        context.Database.Refresh();

        GameUser? updated = context.Database.GetUserByObjectId(player.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Lbp2PlanetsHash.IsBlankHash(), Is.True);
        
        // name
        context.Database.UpdateUserData(updated, new SerializedUpdateData()
        {
            PlanetsHash = "lol"
        }, TokenGame.LittleBigPlanet3);

        context.Database.Refresh();

        planetResponse = client.GetData<ApiAdminUserPlanetsResponse>($"/api/v3/admin/users/name/{updated.Username}/planets");
        Assert.That(planetResponse?.Data, Is.Not.Null);
        Assert.That(planetResponse!.Data!.Lbp3PlanetsHash, Is.EqualTo("lol"));

        resetResponse = await client.DeleteAsync($"/api/v3/admin/users/name/{updated.Username}/planets");
        Assert.That(resetResponse.IsSuccessStatusCode, Is.True);

        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(updated.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Lbp3PlanetsHash.IsBlankHash(), Is.True);
    }

    [Test]
    public async Task ModeratorsMayNotDeleteAdminsAndModerators()
    {
        using TestContext context = this.GetServer();
        GameUser admin = context.CreateUser(role: GameUserRole.Admin);
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        GameUser mod2 = context.CreateUser(role: GameUserRole.Moderator);
        GameUser user = context.CreateUser(role: GameUserRole.User);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);

        // Admin
        HttpResponseMessage response = await client.DeleteAsync($"/api/v3/admin/users/uuid/{admin.UserId}");
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        GameUser? updated = context.Database.GetUserByObjectId(admin.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Admin));

        // Mod
        response = await client.DeleteAsync($"/api/v3/admin/users/uuid/{mod2.UserId}");
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(mod2.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Moderator));

        // User
        response = await client.DeleteAsync($"/api/v3/admin/users/uuid/{user.UserId}");
        Assert.That(response.IsSuccessStatusCode, Is.True);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(user.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Banned));
    }

    [Test]
    public async Task ModeratorsMayNotBanAdminsAndModerators()
    {
        using TestContext context = this.GetServer();
        GameUser admin = context.CreateUser(role: GameUserRole.Admin);
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        GameUser mod2 = context.CreateUser(role: GameUserRole.Moderator);
        GameUser user = context.CreateUser(role: GameUserRole.User);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);
        ApiPunishUserRequest request = new()
        {
            Reason = "lol",
            ExpiryDate = new(2036, 8, 12, 4, 20, 9, 213, new())
        };

        // Admin
        HttpResponseMessage response = await client.PostAsync($"/api/v3/admin/users/uuid/{admin.UserId}/ban", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        GameUser? updated = context.Database.GetUserByObjectId(admin.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Admin));

        // Mod
        response = await client.PostAsync($"/api/v3/admin/users/uuid/{mod2.UserId}/ban", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(mod2.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Moderator));

        // User
        response = await client.PostAsync($"/api/v3/admin/users/uuid/{user.UserId}/ban", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.True);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(user.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Banned));
    }

    [Test]
    public async Task ModeratorsMayNotRestrictAdminsAndModerators()
    {
        using TestContext context = this.GetServer();
        GameUser admin = context.CreateUser(role: GameUserRole.Admin);
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        GameUser mod2 = context.CreateUser(role: GameUserRole.Moderator);
        GameUser user = context.CreateUser(role: GameUserRole.User);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);
        ApiPunishUserRequest request = new()
        {
            Reason = "lol",
            ExpiryDate = new(2036, 8, 12, 4, 20, 9, 213, new())
        };

        // Admin
        HttpResponseMessage response = await client.PostAsync($"/api/v3/admin/users/uuid/{admin.UserId}/restrict", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        GameUser? updated = context.Database.GetUserByObjectId(admin.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Admin));

        // Mod
        response = await client.PostAsync($"/api/v3/admin/users/uuid/{mod2.UserId}/restrict", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(mod2.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Moderator));

        // User
        response = await client.PostAsync($"/api/v3/admin/users/uuid/{user.UserId}/restrict", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.True);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(user.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Restricted));
    }

    [Test]
    public async Task CanOnlyPardonPunishedUsers()
    {
        using TestContext context = this.GetServer();
        GameUser admin = context.CreateUser(role: GameUserRole.Admin);
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        GameUser user = context.CreateUser(role: GameUserRole.User);
        GameUser restricted = context.CreateUser(role: GameUserRole.Restricted);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);

        // Admin
        HttpResponseMessage response = await client.PostAsync($"/api/v3/admin/users/uuid/{admin.UserId}/pardon", null);
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        GameUser? updated = context.Database.GetUserByObjectId(admin.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.Admin));

        // User
        response = await client.PostAsync($"/api/v3/admin/users/uuid/{user.UserId}/pardon", null);
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(user.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.User));

        // Restricted
        response = await client.PostAsync($"/api/v3/admin/users/uuid/{restricted.UserId}/pardon", null);
        Assert.That(response.IsSuccessStatusCode, Is.True);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(restricted.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Role, Is.EqualTo(GameUserRole.User));
    }

    [Test]
    public async Task ModeratorsMayNotResetPasswordOfAdminsAndModerators()
    {
        using TestContext context = this.GetServer();
        GameUser admin = context.CreateUser(role: GameUserRole.Admin);
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        GameUser mod2 = context.CreateUser(role: GameUserRole.Moderator);
        GameUser user = context.CreateUser(role: GameUserRole.User);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);
        ApiResetPasswordRequest request = new()
        {
            PasswordSha512 = HexHelper.BytesToHexString(SHA512.HashData("lmao"u8))
        };

        // Admin
        HttpResponseMessage response = await client.PutAsync($"/api/v3/admin/users/uuid/{admin.UserId}/resetPassword", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        GameUser? updated = context.Database.GetUserByObjectId(admin.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.ShouldResetPassword, Is.False);

        // Mod
        response = await client.PutAsync($"/api/v3/admin/users/uuid/{mod2.UserId}/resetPassword", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.False);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(mod2.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.ShouldResetPassword, Is.False);

        // User
        response = await client.PutAsync($"/api/v3/admin/users/uuid/{user.UserId}/resetPassword", new StringContent(request.AsJson()));
        Assert.That(response.IsSuccessStatusCode, Is.True);
        context.Database.Refresh();

        updated = context.Database.GetUserByObjectId(user.UserId);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.ShouldResetPassword, Is.True);
    }

    [Test]
    public void CanDisallowUsernameUsingApi()
    {
        const string username = "lol";
        using TestContext context = this.GetServer();
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);

        ApiDisallowUsernameRequest disallowRequest = new()
        {
            Username = username,
            Reason = "for the funny",
        };

        // Add via API
        ApiResponse<ApiDisallowedUsernameResponse>? response = client.PostData<ApiDisallowedUsernameResponse>($"/api/v3/admin/disallowed/usernames", disallowRequest, false, false);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(Created));
        Assert.That(response!.Data!.Username, Is.EqualTo(username));
        Assert.That(response!.Data!.Reason, Is.EqualTo(disallowRequest.Reason));
        context.Database.Refresh();

        // Try to add again (different status code)
        response = client.PostData<ApiDisallowedUsernameResponse>($"/api/v3/admin/disallowed/usernames", disallowRequest);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(OK));
        Assert.That(response!.Data!.Username, Is.EqualTo(username));
        Assert.That(response!.Data!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Ensure it's actually in DB
        Assert.That(context.Database.IsUserDisallowed(username), Is.True);
        DisallowedUser? disallowed = context.Database.GetDisallowedUserInfo(username);
        Assert.That(disallowed, Is.Not.Null);
        Assert.That(disallowed!.Username, Is.EqualTo(username));
        Assert.That(disallowed!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Different username will fail
        const string wrong = "Sackface";
        Assert.That(context.Database.IsUserDisallowed(wrong), Is.False);
        Assert.That(context.Database.GetDisallowedUserInfo(wrong), Is.Null);

        // Get list
        ApiListResponse<ApiDisallowedUsernameResponse>? listResponse = client.GetList<ApiDisallowedUsernameResponse>($"/api/v3/admin/disallowed/usernames");
        Assert.That(listResponse?.Data, Is.Not.Null);
        Assert.That(listResponse!.Data![0].Username, Is.EqualTo(username));
        Assert.That(listResponse!.Data![0].Reason, Is.EqualTo(disallowRequest.Reason));

        // Reallow
        ApiDisallowUsernameRequest reallowRequest = new()
        {
            Username = username,
            Reason = "nvm it's not funny",
        };

        response = client.DeleteData<ApiDisallowedUsernameResponse>($"/api/v3/admin/disallowed/usernames", reallowRequest);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(OK));
        context.Database.Refresh();

        // Try to reallow again
        response = client.DeleteData<ApiDisallowedUsernameResponse>($"/api/v3/admin/disallowed/usernames", reallowRequest, false, true);
        Assert.That(response?.Error, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(NotFound));

        // Ensure it's no longer in DB
        Assert.That(context.Database.IsUserDisallowed(disallowRequest.Username), Is.False);
        Assert.That(context.Database.GetDisallowedUserInfo(disallowRequest.Username), Is.Null);
    }

    [Test]
    public void CanDisallowEmailAddressUsingApi()
    {
        const string emailAddress = "guy@stinker.com";
        using TestContext context = this.GetServer();
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);

        ApiDisallowEmailAddressRequest disallowRequest = new()
        {
            Address = emailAddress,
            Reason = "skill issue",
        };

        // Add via API
        ApiResponse<ApiDisallowedEmailAddressResponse>? response = client.PostData<ApiDisallowedEmailAddressResponse>($"/api/v3/admin/disallowed/emailAddresses", disallowRequest, false, false);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(Created));
        Assert.That(response!.Data!.Address, Is.EqualTo(emailAddress));
        Assert.That(response!.Data!.Reason, Is.EqualTo(disallowRequest.Reason));
        context.Database.Refresh();

        // Try to add again (different status code)
        response = client.PostData<ApiDisallowedEmailAddressResponse>($"/api/v3/admin/disallowed/emailAddresses", disallowRequest);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(OK));
        Assert.That(response!.Data!.Address, Is.EqualTo(emailAddress));
        Assert.That(response!.Data!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Ensure it's actually in DB
        Assert.That(context.Database.IsEmailAddressDisallowed(emailAddress), Is.True);
        DisallowedEmailAddress? disallowed = context.Database.GetDisallowedEmailAddressInfo(emailAddress);
        Assert.That(disallowed, Is.Not.Null);
        Assert.That(disallowed!.Address, Is.EqualTo(emailAddress));
        Assert.That(disallowed!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Different address will fail
        const string wrong = "creative@pro.com";
        Assert.That(context.Database.IsEmailAddressDisallowed(wrong), Is.False);
        Assert.That(context.Database.GetDisallowedEmailAddressInfo(wrong), Is.Null);

        // Get list
        ApiListResponse<ApiDisallowedEmailAddressResponse>? listResponse = client.GetList<ApiDisallowedEmailAddressResponse>($"/api/v3/admin/disallowed/emailAddresses");
        Assert.That(listResponse?.Data, Is.Not.Null);
        Assert.That(listResponse!.Data![0].Address, Is.EqualTo(emailAddress));
        Assert.That(listResponse!.Data![0].Reason, Is.EqualTo(disallowRequest.Reason));

        // Reallow
        ApiDisallowEmailAddressRequest reallowRequest = new()
        {
            Address = emailAddress,
            Reason = "no longer has skill issues",
        };

        response = client.DeleteData<ApiDisallowedEmailAddressResponse>($"/api/v3/admin/disallowed/emailAddresses", reallowRequest);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(OK));
        context.Database.Refresh();

        // Try to reallow again
        response = client.DeleteData<ApiDisallowedEmailAddressResponse>($"/api/v3/admin/disallowed/emailAddresses", reallowRequest, false, true);
        Assert.That(response?.Error, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(NotFound));

        // Ensure it's no longer in DB
        Assert.That(context.Database.IsEmailDomainDisallowed(emailAddress), Is.False);
        Assert.That(context.Database.GetDisallowedEmailAddressInfo(emailAddress), Is.Null);
    }

    [Test]
    [TestCase("guy@spam.com")]
    [TestCase("tong@spam.com")]
    [TestCase("spam.com")]
    public void CanDisallowEmailDomainUsingApi(string emailAddress)
    {
        const string emailDomain = "spam.com";
        const string anotherAddress = "test@spam.com";
        using TestContext context = this.GetServer();
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);

        ApiDisallowEmailDomainRequest disallowRequest = new()
        {
            Domain = emailAddress,
            Reason = "skill issue",
        };

        // Add via API
        ApiResponse<ApiDisallowedEmailDomainResponse>? response = client.PostData<ApiDisallowedEmailDomainResponse>($"/api/v3/admin/disallowed/emailDomains", disallowRequest, false, false);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(Created));
        Assert.That(response!.Data!.Domain, Is.EqualTo(emailDomain));
        Assert.That(response!.Data!.Reason, Is.EqualTo(disallowRequest.Reason));
        context.Database.Refresh();

        // Try to add again (different status code)
        response = client.PostData<ApiDisallowedEmailDomainResponse>($"/api/v3/admin/disallowed/emailDomains", disallowRequest);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(OK));
        Assert.That(response!.Data!.Domain, Is.EqualTo(emailDomain));
        Assert.That(response!.Data!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Ensure it's actually in DB
        Assert.That(context.Database.IsEmailDomainDisallowed(emailAddress), Is.True);
        DisallowedEmailDomain? disallowed = context.Database.GetDisallowedEmailDomainInfo(emailAddress);
        Assert.That(disallowed, Is.Not.Null);
        Assert.That(disallowed!.Domain, Is.EqualTo(emailDomain));
        Assert.That(disallowed!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Ensure we can get it using another address aswell
        Assert.That(context.Database.IsEmailDomainDisallowed(anotherAddress), Is.True);
        disallowed = context.Database.GetDisallowedEmailDomainInfo(anotherAddress);
        Assert.That(disallowed, Is.Not.Null);
        Assert.That(disallowed!.Domain, Is.EqualTo(emailDomain));
        Assert.That(disallowed!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Different address will fail
        const string wrong = "gähmieng@pro.com";
        Assert.That(context.Database.GetDisallowedEmailDomainInfo(wrong), Is.Null);
        Assert.That(context.Database.IsEmailDomainDisallowed(wrong), Is.False);

        // Get list
        ApiListResponse<ApiDisallowedEmailDomainResponse>? listResponse = client.GetList<ApiDisallowedEmailDomainResponse>($"/api/v3/admin/disallowed/emailDomains");
        Assert.That(listResponse?.Data, Is.Not.Null);
        Assert.That(listResponse!.Data![0].Domain, Is.EqualTo(emailDomain));
        Assert.That(listResponse!.Data![0].Reason, Is.EqualTo(disallowRequest.Reason));

        // Reallow
        ApiDisallowEmailDomainRequest reallowRequest = new()
        {
            Domain = emailAddress,
            Reason = "idk",
        };

        response = client.DeleteData<ApiDisallowedEmailDomainResponse>($"/api/v3/admin/disallowed/emailDomains", reallowRequest);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(OK));
        context.Database.Refresh();

        // Try to reallow again
        response = client.DeleteData<ApiDisallowedEmailDomainResponse>($"/api/v3/admin/disallowed/emailDomains", reallowRequest, false, true);
        Assert.That(response?.Error, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(NotFound));

        // Ensure it's no longer in DB
        Assert.That(context.Database.IsEmailDomainDisallowed(emailAddress), Is.False);
        Assert.That(context.Database.GetDisallowedEmailDomainInfo(emailAddress), Is.Null);

        Assert.That(context.Database.IsEmailDomainDisallowed(anotherAddress), Is.False);
        Assert.That(context.Database.GetDisallowedEmailDomainInfo(anotherAddress), Is.Null);
    }

    [Test]
    [TestCase(GameUserRole.User)]
    [TestCase(GameUserRole.Curator)]
    public void CannotDisallowContentAsNonMod(GameUserRole role)
    {
        using TestContext context = this.GetServer();
        GameUser moron = context.CreateUser(role: role);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, moron);

        // Domain
        ApiDisallowEmailDomainRequest disallowDomainRequest = new()
        {
            Domain = "gmail.com",
            Reason = "haha get rekt everyone",
        };

        ApiResponse<ApiDisallowedEmailDomainResponse>? domainResponse = client.PostData<ApiDisallowedEmailDomainResponse>($"/api/v3/admin/disallowed/emailDomains", disallowDomainRequest, false, true);
        Assert.That(domainResponse?.Data, Is.Null);

        context.Database.Refresh();
        Assert.That(context.Database.IsEmailDomainDisallowed(disallowDomainRequest.Domain), Is.False);
        Assert.That(context.Database.GetDisallowedEmailDomainInfo(disallowDomainRequest.Domain), Is.Null);

        // Address
        ApiDisallowEmailAddressRequest disallowAddressRequest = new()
        {
            Address = "idk@kdi.idk",
            Reason = "idk",
        };

        ApiResponse<ApiDisallowedEmailAddressResponse>? addressResponse = client.PostData<ApiDisallowedEmailAddressResponse>($"/api/v3/admin/disallowed/emailAddresses", disallowAddressRequest, false, true);
        Assert.That(addressResponse?.Data, Is.Null);

        context.Database.Refresh();
        Assert.That(context.Database.IsEmailAddressDisallowed(disallowAddressRequest.Address), Is.False);
        Assert.That(context.Database.GetDisallowedEmailAddressInfo(disallowAddressRequest.Address), Is.Null);

        // Name
        ApiDisallowUsernameRequest disallowNameRequest = new()
        {
            Username = "EpicScrewdriver2",
            Reason = "screw you in particular",
        };

        ApiResponse<ApiDisallowedUsernameResponse>? nameResponse = client.PostData<ApiDisallowedUsernameResponse>($"/api/v3/admin/disallowed/usernames", disallowNameRequest, false, true);
        Assert.That(nameResponse?.Data, Is.Null);

        context.Database.Refresh();
        Assert.That(context.Database.IsUserDisallowed(disallowNameRequest.Username), Is.False);
        Assert.That(context.Database.GetDisallowedUserInfo(disallowNameRequest.Username), Is.Null);
    }
}