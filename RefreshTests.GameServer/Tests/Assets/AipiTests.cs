using System.Reflection;
using System.Security.Cryptography;
using Bunkum.Core.Storage;
using Newtonsoft.Json;
using Refresh.Common.Helpers;
using Refresh.Core.Helpers;
using Refresh.Core.Importing;
using Refresh.Core.Services;
using Refresh.Core.Types.Assets.Validation;
using Refresh.Core.Types.Data;
using Refresh.Database.Models.Assets;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Users;
using RefreshTests.GameServer.AipiServer;
using RefreshTests.GameServer.GameServer.Services;

namespace RefreshTests.GameServer.Tests.Assets;

public class AipiTests : GameServerTest
{
    private static readonly byte[] TestAsset = ResourceHelper.ReadResource("RefreshTests.GameServer.Resources.1x1.png", Assembly.GetExecutingAssembly());
    
    [Test]
    public void TestEndpointsWork()
    {
        using TestContext context = this.GetServer();
        TestRefreshGameServer server = context.Server.Value;
        server.Server.AddEndpointGroup<TestAipiEndpoints>();
        
        HttpResponseMessage message = context.Http.GetAsync("/").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));
        Assert.That(message.Content.ReadAsStringAsync().Result, Is.EqualTo("AIPI scanning service"));
        
        message = context.Http.PostAsync("/eva/predict", new StreamContent(new MemoryStream(TestAsset))).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));
        
        AipiResponse<Dictionary<string, float>>? result = JsonConvert.DeserializeObject<AipiResponse<Dictionary<string, float>>>(message.Content.ReadAsStringAsync().Result);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.True);
        Assert.That(result!.Reason, Is.Null);
        Assert.That(result!.Data, Is.Not.Null);
        Assert.That(result!.Data!.Count, Is.EqualTo(2));
        Assert.That(result!.Data!.GetValueOrDefault("sixSeven"), Is.EqualTo(67.0f));
        Assert.That(result!.Data!.GetValueOrDefault("hi"), Is.EqualTo(123.456f));
        
        // Run again but with a threshold that will exclude tag sixSeven
        message = context.Http.PostAsync("/eva/predict?threshold=70.0", new StreamContent(new MemoryStream(TestAsset))).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));
        
        result = JsonConvert.DeserializeObject<AipiResponse<Dictionary<string, float>>>(message.Content.ReadAsStringAsync().Result);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.True);
        Assert.That(result!.Reason, Is.Null);
        Assert.That(result!.Data, Is.Not.Null);
        Assert.That(result!.Data!.Count, Is.EqualTo(1));
        Assert.That(result!.Data!.ContainsKey("sixSeven"), Is.False); // was excluded because probability is too low
        Assert.That(result!.Data!.GetValueOrDefault("hi"), Is.EqualTo(123.456f));
        
        // Now force a failure
        context.Http.DefaultRequestHeaders.Add("X-ForcedFailureReason", "real");
        message = context.Http.PostAsync("/eva/predict?threshold=70.0", new StreamContent(new MemoryStream(TestAsset))).Result;
        Assert.That(message.StatusCode, Is.EqualTo(NotAcceptable));
        
        result = JsonConvert.DeserializeObject<AipiResponse<Dictionary<string, float>>>(message.Content.ReadAsStringAsync().Result);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Success, Is.False);
        Assert.That(result!.Reason, Is.Not.Null);
        Assert.That(result!.Reason, Is.EqualTo("real"));
        Assert.That(result!.Data, Is.Null);
    }

    private bool ScanImage(float threshold, string[] bannedTags, TestContext context, GameUser? uploader = null, bool autoRestrict = false)
    {
        IDataStore dataStore = context.GetDataStore();
        string hash = BitConverter.ToString(SHA1.HashData(TestAsset)).Replace("-", "").ToLower();
        // Can't expect AipiService's ImageImporter to copy the PNG (or save the converted PNG if we weren't using a PNG)
        // because Bunkum's InMemoryDataStore stubs OpenWriteStream() by throwing a NotImplementedException
        dataStore.WriteToStore("png/" + hash, TestAsset);        

        TestAipiService aipi = context.InitAipi(threshold, bannedTags, autoRestrict);
        
        uploader ??= context.CreateUser();
        GameAsset metadata = new()
        {
            AssetHash = hash,
            AssetType = GameAssetType.Png,
            OriginalUploader = uploader,
            SizeInBytes = TestAsset.Length,
            IsPSP = false,
        };
        return aipi.ScanAndHandleAsset(context.Database, dataStore, metadata, uploader);
    }

    [Test]
    public void ImageIsIgnoredIfNoKnownTagsReturned()
    {
        using TestContext context = this.GetServer();
        
        Assert.That(this.ScanImage(0.0f, [], context), Is.False);
    }

    [Test]
    public void ImageIsIgnoredIfBackendFailure()
    {
        using TestContext context = this.GetServer();
        context.Http.DefaultRequestHeaders.Add("X-ForcedFailureReason", "lel");

        bool hasThrown = false;
        try
        {
            this.ScanImage(0.0f, [], context);
        }
        catch (Exception ex)
        {
            hasThrown = true;
            Assert.That(ex.Message, Is.EqualTo($"One or more errors occurred. (NotAcceptable: lel)"));
        }
        Assert.That(hasThrown, Is.True);
    }

    [Test]
    [TestCase("sixSeven", false)]
    [TestCase("hi", true)]
    public void ImageIsFlaggedIfAnyKnownTagsReturned(string tagName, bool flaggedOnHigherProbability)
    {
        using TestContext context = this.GetServer();
        
        Assert.That(this.ScanImage(0.0f, [tagName], context), Is.True);
        
        // Now raise the probability, so that sixSeven won't appear anymore
        Assert.That(this.ScanImage(90.0f, [tagName], context), Is.EqualTo(flaggedOnHigherProbability));
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public void UserGetsAutoRestrictedIfWanted(bool autoRestrict)
    {
        using TestContext context = this.GetServer();
        GameUser uploader = context.CreateUser();
        
        Assert.That(this.ScanImage(0.0f, ["hi"], context, uploader, autoRestrict), Is.True);
        
        // re-get user from DB to check their role
        GameUser? updatedUser = context.Database.GetUserByObjectId(uploader.UserId);
        Assert.That(updatedUser, Is.Not.Null);
        
        if (autoRestrict)
            Assert.That(updatedUser!.Role, Is.EqualTo(GameUserRole.Restricted));
        else
            Assert.That(updatedUser!.Role, Is.GreaterThan(GameUserRole.Restricted));
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public void ImageGetsAipiScannedByReferenceValidator(bool mustBeTexture) // param value should not matter
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet1, TokenPlatform.PS3, out Token token, user);

        DataContext dataContext = context.GetDataContext(token);
        TestAipiService aipi = context.InitAipi(0.0f, ["hi"], false);
        AssetImporter importer = new(dataContext.Logger, context.Time);

        ReadOnlySpan<byte> data = "TEX "u8;
        string hash = BitConverter.ToString(SHA1.HashData(data)).Replace("-", "").ToLower();
        dataContext.DataStore.WriteToStore(hash, data);

        ValidatedAssetResult result = ResourceValidationHelper.ValidateReference(new(hash, dataContext, importer, aipi)
        {
            MustBeTexture = mustBeTexture,
        }, dataContext.Logger);
        
        Assert.That(result.Status, Is.EqualTo(UnprocessableContent));
        Assert.That(result.NewAssetRef, Is.EqualTo("0"));
        Assert.That(result.AssetInfo, Is.Not.Null);
        Assert.That(result.ExistsInDataStore, Is.True);
    }
}