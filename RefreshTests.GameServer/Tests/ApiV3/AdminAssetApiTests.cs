using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Users;
using RefreshTests.GameServer.Extensions;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Request;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes.Response.Data;
using Refresh.Database.Models.Assets;

namespace RefreshTests.GameServer.Tests.ApiV3;

public class AdminAssetApiTests : GameServerTest
{
    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void CanDisallowAssetUsingApi(bool uploadGameAsset, bool specifyType)
    {
        const string hash = "hashie";
        using TestContext context = this.GetServer();
        GameUser mod = context.CreateUser(role: GameUserRole.Moderator);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, mod);

        if (uploadGameAsset)
        {
            context.Database.AddAssetToDatabase(new()
            {
                AssetHash = hash,
                AssetType = GameAssetType.LocalProfile,
            });
            context.Database.Refresh();
        }

        ApiDisallowAssetRequest disallowRequest = new()
        {
            AssetHash = hash,
            AssetType = specifyType ? nameof(GameAssetType.VoiceRecording) : null,
            Reason = "garbage singing to torture people",
        };

        // Add via API
        ApiResponse<ApiDisallowedAssetResponse>? response = client.PostData<ApiDisallowedAssetResponse>($"/api/v3/admin/disallowed/assets", disallowRequest, false, false);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(Created));
        Assert.That(response!.Data!.AssetHash, Is.EqualTo(hash));
        if (!specifyType)
        {
            if (!uploadGameAsset) Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.Unknown));
            else Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.LocalProfile));
        }
        else
        {
            Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.VoiceRecording));
        }
        Assert.That(response!.Data!.Reason, Is.EqualTo(disallowRequest.Reason));
        context.Database.Refresh();

        // Try to add again (different status code)
        response = client.PostData<ApiDisallowedAssetResponse>($"/api/v3/admin/disallowed/assets", disallowRequest);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(OK));
        Assert.That(response!.Data!.AssetHash, Is.EqualTo(hash));
        Assert.That(response!.Data!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Ensure it's actually in DB
        DisallowedAsset? disallowed = context.Database.GetDisallowedAssetInfo(hash);
        Assert.That(disallowed, Is.Not.Null);
        Assert.That(disallowed!.AssetHash, Is.EqualTo(hash));
        if (!specifyType)
        {
            if (!uploadGameAsset) Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.Unknown));
            else Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.LocalProfile));
        }
        else
        {
            Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.VoiceRecording));
        }
        Assert.That(disallowed!.Reason, Is.EqualTo(disallowRequest.Reason));

        // Different hash will fail
        const string wrong = "ass that info is null";
        Assert.That(context.Database.GetDisallowedAssetInfo(wrong), Is.Null);

        // Get list
        ApiListResponse<ApiDisallowedAssetResponse>? listResponse = client.GetList<ApiDisallowedAssetResponse>($"/api/v3/admin/disallowed/assets");
        Assert.That(listResponse?.Data, Is.Not.Null);
        Assert.That(listResponse!.Data![0].AssetHash, Is.EqualTo(hash));
        if (!specifyType)
        {
            if (!uploadGameAsset) Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.Unknown));
            else Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.LocalProfile));
        }
        else
        {
            Assert.That(response!.Data!.AssetType, Is.EqualTo(GameAssetType.VoiceRecording));
        }
        Assert.That(listResponse!.Data![0].Reason, Is.EqualTo(disallowRequest.Reason));

        // Reallow
        ApiDisallowAssetRequest reallowRequest = new()
        {
            AssetHash = hash,
            Reason = "shit i pasted in the wrong hash sorry",
        };

        response = client.DeleteData<ApiDisallowedAssetResponse>($"/api/v3/admin/disallowed/assets", reallowRequest);
        Assert.That(response?.Data, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(OK));
        context.Database.Refresh();

        // Try to reallow again
        response = client.DeleteData<ApiDisallowedAssetResponse>($"/api/v3/admin/disallowed/assets", reallowRequest, false, true);
        Assert.That(response?.Error, Is.Not.Null);
        Assert.That(response!.StatusCode, Is.EqualTo(NotFound));

        // Ensure it's no longer in DB
        Assert.That(context.Database.GetDisallowedAssetInfo(hash), Is.Null);
    }

    [Test]
    [TestCase(GameUserRole.User)]
    [TestCase(GameUserRole.Curator)]
    public void CannotDisallowAssetsAsNonMod(GameUserRole role)
    {
        using TestContext context = this.GetServer();
        GameUser moron = context.CreateUser(role: role);
        HttpClient client = context.GetAuthenticatedClient(TokenType.Api, moron);

        // Asset
        ApiDisallowAssetRequest disallowAssetRequest = new()
        {
            AssetHash = "ass type and hash",
            AssetType = nameof(GameAssetType.Game),
            Reason = "Game Disappeared.",
        };

        ApiResponse<ApiDisallowedAssetResponse>? assetResponse = client.PostData<ApiDisallowedAssetResponse>($"/api/v3/admin/disallowed/assets", disallowAssetRequest, false, true);
        Assert.That(assetResponse?.Data, Is.Null);

        context.Database.Refresh();
        Assert.That(context.Database.GetDisallowedAssetInfo(disallowAssetRequest.AssetHash), Is.Null);
    }
}