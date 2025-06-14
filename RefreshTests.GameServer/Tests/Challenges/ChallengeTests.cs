using Refresh.Database;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Levels.Challenges;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.Game.Types.Challenges.LbpHub;
using Refresh.Interfaces.Game.Types.Lists;
using RefreshTests.GameServer.Extensions;

namespace RefreshTests.GameServer.Tests.Challenges;

public class ChallengeTests : GameServerTest
{
    [Test]
    public void BlockChallengeUploadWithNoCriteria()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameLevel level = context.CreateLevel(user);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user);

        // Try to create invalid challenge (one with no criteria)
        SerializedChallenge challengeToUpload = new()
        {
            Level = new SerializedChallengeLevel
            {
                LevelId = level.LevelId,
                Type = level.SlotType.ToGameType(),
            },
            Name = "Test Challenge",
            StartCheckpointUid = 1,
            FinishCheckpointUid = 2,
            ExpiresAt = 3,
        };

        HttpResponseMessage message = client.PostAsync($"/lbp/challenge", new StringContent(challengeToUpload.AsXML())).Result;
        Assert.That(message.StatusCode, Is.EqualTo(BadRequest));

        // make sure the attempted challenge upload is not there
        DatabaseList<GameChallenge> challenges = context.Database.GetChallenges(0, 5);
        Assert.That(challenges.TotalItems, Is.Zero);
        Assert.That(challenges.Items, Is.Empty);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    public void UploadChallengeWithCriterion(byte criterionType)
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameLevel level = context.CreateLevel(user);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user);

        SerializedChallenge challengeToUpload = new()
        {
            Level = new SerializedChallengeLevel
            {
                LevelId = level.LevelId,
                Type = level.SlotType.ToGameType(),
            },
            Name = "Test Challenge",
            StartCheckpointUid = 1,
            FinishCheckpointUid = 2,
            ExpiresAt = 3,
            Criteria = 
            [
                new SerializedChallengeCriterion
                {
                    Type = criterionType,
                    Value = 0,
                }
            ]
        };

        HttpResponseMessage message = client.PostAsync($"/lbp/challenge", new StringContent(challengeToUpload.AsXML())).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        // make sure the attempted challenge upload is there
        DatabaseList<GameChallenge> challenges = context.Database.GetChallenges(0, 5);
        Assert.That(challenges.TotalItems, Is.EqualTo(1));
        Assert.That((byte)challenges.Items.First().Type, Is.EqualTo(criterionType));
    }

    [Test]
    public void BlockChallengeUploadToInvalidLevel()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user);

        SerializedChallenge challengeToUpload = new()
        {
            Level = new SerializedChallengeLevel
            {
                LevelId = 123454678,
                Type = "user",
            },
            Name = "Test Challenge",
            StartCheckpointUid = 1,
            FinishCheckpointUid = 2,
            ExpiresAt = 3,
            Criteria = 
            [
                new SerializedChallengeCriterion()
            ]
        };

        HttpResponseMessage message = client.PostAsync($"/lbp/challenge", new StringContent(challengeToUpload.AsXML())).Result;
        Assert.That(message.StatusCode, Is.EqualTo(NotFound));
    }

    [Test]
    public void GetChallengesByUserAndFilter()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameLevel level = context.CreateLevel(user);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user);

        for (int i = 0; i < 10; i++)
        {
            context.Database.CreateChallenge(new SerializedChallenge() 
            {
                Name = $"Test Challenge Number {i}",
                StartCheckpointUid = 1,
                FinishCheckpointUid = 2,
                ExpiresAt = -5 + i,
                Criteria = 
                [
                    new SerializedChallengeCriterion
                    {
                        Type = 2,
                        Value = 0,
                    }
                ]
            }, level, user);
        }

        // Check for all challenges
        HttpResponseMessage message = client.GetAsync($"/lbp/user/{user.Username}/challenges").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));
        SerializedChallengeList result = message.Content.ReadAsXML<SerializedChallengeList>();
        Assert.That(result.Items, Has.Count.EqualTo(10));

        // Check for active challenges
        message = client.GetAsync($"/lbp/user/{user.Username}/challenges?status=active").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));
        result = message.Content.ReadAsXML<SerializedChallengeList>();
        Assert.That(result.Items, Has.Count.EqualTo(4));

        // Check for expired challenges
        message = client.GetAsync($"/lbp/user/{user.Username}/challenges?status=expired").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));
        result = message.Content.ReadAsXML<SerializedChallengeList>();
        Assert.That(result.Items, Has.Count.EqualTo(6));
    }

    [Test]
    public void GetChallengesNotByUser()
    {
        using TestContext context = this.GetServer();
        GameUser user1 = context.CreateUser();
        GameUser user2 = context.CreateUser();
        GameLevel level = context.CreateLevel(user1);
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, user1);

        for (int i = 0; i < 10; i++)
        {
            context.Database.CreateChallenge(new SerializedChallenge() 
            {
                Name = $"Test Challenge Number {i}",
                StartCheckpointUid = 1,
                FinishCheckpointUid = 2,
                ExpiresAt = 7,
                Criteria = 
                [
                    new SerializedChallengeCriterion
                    {
                        Type = 2,
                        Value = 0,
                    }
                ]
            }, level, user2);
        }

        // Create challenge by the calling user for comparison
        context.Database.CreateChallenge(new SerializedChallenge() 
        {
            Name = $"BONUS Challenge",
            StartCheckpointUid = 1,
            FinishCheckpointUid = 3,
            ExpiresAt = 999,
            Criteria = 
            [
                new SerializedChallengeCriterion
                {
                    Type = 4,
                    Value = 0,
                }
            ]
        }, level, user1);

        // Check for all challenges not by user1
        HttpResponseMessage message = client.GetAsync($"/lbp/user/{user1.Username}/friends/challenges").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));
        SerializedChallengeList result = message.Content.ReadAsXML<SerializedChallengeList>();
        Assert.That(result.Items, Has.Count.EqualTo(10));

        // Check for truly all challenges
        message = client.GetAsync($"/lbp/user/nouser/friends/challenges").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));
        result = message.Content.ReadAsXML<SerializedChallengeList>();
        Assert.That(result.Items, Has.Count.EqualTo(11));
    }
}