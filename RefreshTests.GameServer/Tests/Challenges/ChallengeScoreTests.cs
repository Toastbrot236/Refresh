using System.Security.Cryptography;
using Refresh.Database;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Levels.Challenges;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.Game.Types.Challenges.LbpHub;
using RefreshTests.GameServer.Extensions;

namespace RefreshTests.GameServer.Tests.Challenges;

public class ChallengeScoreTests : GameServerTest
{
    [Test]
    public void SubmitValidScores()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameLevel level = context.CreateLevel(user);
        GameChallenge challenge = context.CreateChallenge(user, level);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user);

        void SubmitScore(ReadOnlySpan<byte> ghostAssetData)
        {
            string hash = BitConverter.ToString(SHA1.HashData(ghostAssetData))
                .Replace("-", "")
                .ToLower();

            HttpResponseMessage response = client.PostAsync("/lbp/upload/" + hash, new ByteArrayContent(ghostAssetData.ToArray())).Result;
            Assert.That(response.StatusCode, Is.EqualTo(OK));

            // Upload the actual score
            SerializedChallengeAttempt attempt = new()
            {
                Score = 1234567890,
                GhostHash = hash,
            };
            HttpResponseMessage message = client.PostAsync($"/lbp/challenge/{challenge.ChallengeId}/scoreboard", new StringContent(attempt.AsXML())).Result;
            Assert.That(message.StatusCode, Is.EqualTo(OK));
        }

        // First score is allowed to have duplicate finish checkpoints
        SubmitScore("""<ghost><checkpoint uid="1" time="11"></checkpoint><checkpoint uid="2" time="22"></checkpoint><checkpoint uid="2" time="33"></checkpoint></ghost>"""u8);
        // More unique checkpoints + not in correct order
        SubmitScore("""<ghost><checkpoint uid="2" time="3"></checkpoint><checkpoint uid="1" time="1"></checkpoint><checkpoint uid="3" time="2"></checkpoint></ghost>"""u8);
    }

    [Test]
    public void BlockInvalidScores()
    {
        using TestContext context = this.GetServer();
        GameUser user1 = context.CreateUser();
        GameLevel level = context.CreateLevel(user1);
        GameChallenge challenge = context.CreateChallenge(user1, level);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user1);

        void BlockScore(ReadOnlySpan<byte> ghostAssetData)
        {
            string hash = BitConverter.ToString(SHA1.HashData(ghostAssetData))
                .Replace("-", "")
                .ToLower();

            HttpResponseMessage response = client.PostAsync("/lbp/upload/" + hash, new ByteArrayContent(ghostAssetData.ToArray())).Result;
            Assert.That(response.StatusCode, Is.EqualTo(OK));

            // Upload the actual score
            SerializedChallengeAttempt attempt = new()
            {
                Score = 1234567890,
                GhostHash = hash,
            };
            HttpResponseMessage message = client.PostAsync($"/lbp/challenge/{challenge.ChallengeId}/scoreboard", new StringContent(attempt.AsXML())).Result;
            Assert.That(message.StatusCode, Is.EqualTo(BadRequest));
        }

        BlockScore("""<ghost>"""u8);
        BlockScore("""<ghost></ghost>"""u8);
        BlockScore("""<ghost>aaaaaaaaaaaaa</ghost>"""u8);
        // invalid checkpoint uids
        BlockScore("""<ghost><checkpoint uid="5" time="11"></checkpoint><checkpoint uid="4" time="22"></checkpoint></ghost>"""u8); 
        // duplicate finish checkpoint on score which isn't the first one
        BlockScore("""<ghost>checkpoint uid="1" time="4"></checkpoint><checkpoint uid="2" time="11"></checkpoint><checkpoint uid="2" time="22"></checkpoint></ghost>"""u8);
    }

    [Test]
    public void BlockScoreForInvalidChallenge()
    {
        using TestContext context = this.GetServer();
        GameUser user1 = context.CreateUser();
        GameLevel level = context.CreateLevel(user1);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user1);

        ReadOnlySpan<byte> ghostAssetData = """<ghost>"""u8;
        string hash = BitConverter.ToString(SHA1.HashData(ghostAssetData))
            .Replace("-", "")
            .ToLower();

        HttpResponseMessage response = client.PostAsync("/lbp/upload/" + hash, new ByteArrayContent(ghostAssetData.ToArray())).Result;
        Assert.That(response.StatusCode, Is.EqualTo(OK));

        // Upload the actual score
        SerializedChallengeAttempt attempt = new()
        {
            Score = 1234567890,
            GhostHash = hash,
        };
        HttpResponseMessage message = client.PostAsync($"/lbp/challenge/756432/scoreboard", new StringContent(attempt.AsXML())).Result;
        Assert.That(message.StatusCode, Is.EqualTo(NotFound));
    }

    [Test]
    public void GetUsersHighScores()
    {
        using TestContext context = this.GetServer();
        GameUser user1 = context.CreateUser();
        GameLevel level = context.CreateLevel(user1);
        GameChallenge challenge = context.CreateChallenge(user1, level);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user1);

        // get no score
        HttpResponseMessage message = client.GetAsync($"/lbp/challenge/{challenge.ChallengeId}/scoreboard/{user1.Username}").Result;
        Assert.That(message.StatusCode, Is.EqualTo(NotFound));

        // get first score
        context.SubmitChallengeScore(456, "real", challenge, user1);

        message = client.GetAsync($"/lbp/challenge/{challenge.ChallengeId}/scoreboard/{user1.Username}").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        SerializedChallengeScore result = message.Content.ReadAsXML<SerializedChallengeScore>();
        Assert.That(result.Score, Is.EqualTo(456));
        Assert.That(result.GhostHash, Is.EqualTo("real"));

        // still get first score
        context.SubmitChallengeScore(123, "realer", challenge, user1);

        message = client.GetAsync($"/lbp/challenge/{challenge.ChallengeId}/scoreboard/{user1.Username}").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        result = message.Content.ReadAsXML<SerializedChallengeScore>();
        Assert.That(result.Score, Is.EqualTo(456));
        Assert.That(result.GhostHash, Is.EqualTo("real"));

        // get third score
        context.SubmitChallengeScore(789, "realest", challenge, user1);

        message = client.GetAsync($"/lbp/challenge/{challenge.ChallengeId}/scoreboard/{user1.Username}").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        result = message.Content.ReadAsXML<SerializedChallengeScore>();
        Assert.That(result.Score, Is.EqualTo(789));
        Assert.That(result.GhostHash, Is.EqualTo("realest"));
    }
}