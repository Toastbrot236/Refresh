using Refresh.Core.Configuration;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request.Authentication;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Users;
using Refresh.Interfaces.Game.Types.UserData;
using RefreshTests.GameServer.Extensions;

namespace RefreshTests.GameServer.Tests.Registration;

public class RegistrationByCodeTests : GameServerTest
{
    private const string Username = "ShadowSlayer_69";
    private const string Email = "xx_shadowslayer_xx@real.legit";
    private const string PasswordSha512 = "ed71cd7b6d39f762ececdd307b18badaa81f920a51ac0f1e5e7376d0a739f703acc736709ecb7111ce2013d187a2a02ae3c47ed7ca9208ccfa9487745a681f07";

    private void PrepareConfig(TestContext context)
    {
        GameServerConfig config = context.Server.Value.GameServerConfig;
        config.RequireGameLoginToRegister = true;
        config.EnableGuestAccounts = true;
    }

    [Test]
    public void RegisterSuccessfully()
    {
        using TestContext context = this.GetServer();
        this.PrepareConfig(context);

        // Fake game auth call
        GameUser guest = context.Database.CreateGuestUser(Username, TokenPlatform.PS3);

        // Ensure platform auth options are defaulted correctly
        Assert.That(guest.PsnAuthenticationAllowed, Is.True);
        Assert.That(guest.RpcnAuthenticationAllowed, Is.False);
        Assert.That(guest.AllowIpAuthentication, Is.False);

        // Ensure the user is a guest with a registration code
        Assert.That(guest.EmailAddressVerified, Is.False);
        Assert.That(guest.Role, Is.EqualTo(GameUserRole.Guest));
        Assert.That(guest.RegistrationCode, Is.Not.Null);

        string regCode = guest.RegistrationCode!;
        context.Database.Refresh();

        // Ensure user can be fetched by code
        GameUser? gottenByCodeSearch = context.Database.GetUserByRegistrationCode(regCode);
        Assert.That(gottenByCodeSearch, Is.Not.Null);
        Assert.That(gottenByCodeSearch!.UserId, Is.EqualTo(guest.UserId));

        context.Database.Refresh();

        // Register through API
        ApiResponse<ApiAuthenticationResponse>? response = context.Http.PostData<ApiAuthenticationResponse>("/api/v3/register/code", new ApiRegisterByCodeRequest
        {
            Code = regCode,
            EmailAddress = Email,
            PasswordSha512 = PasswordSha512,
        });
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Data, Is.Not.Null);
        Assert.That(response.Data!.UserId, Is.EqualTo(guest.UserId.ToString()));

        // User can no longer be fetched by registration code
        Assert.That(context.Database.GetUserByRegistrationCode(regCode), Is.Null);

        // User can be fetched by username, they're no longer a guest and they
        // don't have a code anymore
        GameUser? user = context.Database.GetUserByUsername(Username);
        Assert.That(user, Is.Not.Null);
        Assert.That(user!.UserId, Is.EqualTo(guest.UserId));
        Assert.That(user.Role, Is.EqualTo(GameUserRole.User));
        Assert.That(user.RegistrationCode, Is.Null);
    }

    [Test]
    public void RegistrationFailsIfNoGuest()
    {
        using TestContext context = this.GetServer();
        this.PrepareConfig(context);

        ApiResponse<ApiAuthenticationResponse>? response = context.Http.PostData<ApiAuthenticationResponse>("/api/v3/register/code", new ApiRegisterByCodeRequest
        {
            Code = "123456",
            EmailAddress = Email,
            PasswordSha512 = PasswordSha512,
        }, false, true);
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Data, Is.Null);
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error!.StatusCode, Is.EqualTo(Forbidden));
    }

    [Test]
    public void RegistrationFailsIfInvalidCode()
    {
        using TestContext context = this.GetServer();
        this.PrepareConfig(context);

        GameUser guest = context.Database.CreateGuestUser(Username, TokenPlatform.PS3);
        Assert.That(guest.RegistrationCode, Is.Not.Null);

        ApiResponse<ApiAuthenticationResponse>? response = context.Http.PostData<ApiAuthenticationResponse>("/api/v3/register/code", new ApiRegisterByCodeRequest
        {
            Code = guest.RegistrationCode + "1",
            EmailAddress = Email,
            PasswordSha512 = PasswordSha512,
        }, false, true);
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Data, Is.Null);
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error!.StatusCode, Is.EqualTo(Forbidden));
    }

    [Test]
    public void CantSignIntoApiAsGuest()
    {
        using TestContext context = this.GetServer();
        this.PrepareConfig(context);

        GameUser guest = context.Database.CreateGuestUser(Username, TokenPlatform.PS3);

        ApiResponse<ApiAuthenticationResponse>? response = context.Http.PostData<ApiAuthenticationResponse>("/api/v3/login", new ApiAuthenticationRequest
        {
            EmailAddress = "",
            PasswordSha512 = PasswordSha512,
        }, false, true);
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Data, Is.Null);
        Assert.That(response.Error, Is.Not.Null);
        Assert.That(response.Error!.StatusCode, Is.EqualTo(Forbidden));
    }

    [Test]
    public void CantUpdateProfileIngameAsGuest()
    {
        using TestContext context = this.GetServer();
        this.PrepareConfig(context);

        GameUser guest = context.Database.CreateGuestUser(Username, TokenPlatform.PS3);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet2, TokenPlatform.PS3, out string _, guest);

        SerializedUpdateDataProfile request = new()
        {
            Description = "Hi",
        };
        HttpResponseMessage message = client.PostAsync($"/lbp/updateUser", new StringContent(request.AsXML())).Result;
        Assert.That(message.StatusCode, Is.EqualTo(BadRequest));
    }

    [Test]
    public void EnsureUpdatedCodeIsDifferent()
    {
        using TestContext context = this.GetServer();
        this.PrepareConfig(context);

        GameUser guest = context.Database.CreateGuestUser(Username, TokenPlatform.PS3);
        Assert.That(guest.RegistrationCode, Is.Not.Null);

        string code = guest.RegistrationCode!;

        guest = context.Database.UpdateGuestUser(guest);
        Assert.That(guest.RegistrationCode, Is.Not.Null);
        Assert.That(guest.RegistrationCode, Is.Not.EqualTo(code));
    }
}