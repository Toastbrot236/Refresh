using Refresh.Database.Models.Activity;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;

namespace RefreshTests.GameServer.Tests.Events;

public class EventVisibilityTests : GameServerTest
{
    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public void ShowOrHideCustomEvents(bool isGame)
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameLevel level = context.CreateLevel(user);

        // Create game events
        Event ev = context.Database.CreateEvent(user, new()
        {
            Actor = user,
            EventType = EventType.UserFavourite,
            OverType = EventOverType.Activity,
        });
        context.Database.CreateEvent(user, new()
        {
            Actor = user,
            EventType = EventType.UserUnfavourite,
            OverType = EventOverType.Activity,
        });

        // Create custom events
        context.Database.CreateEvent(user, new()
        {
            Actor = user,
            EventType = EventType.UpdateUser,
            OverType = EventOverType.Activity,
        });
        context.Database.CreateEvent(user, new()
        {
            Actor = user,
            EventType = EventType.UpdateUser,
            OverType = EventOverType.Activity,
        });

        context.Database.Refresh();

        // Now count the events returned
        DatabaseActivityPage page = context.Database.GetGlobalRecentActivityPage(new()
        {
            Timestamp = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds(),
            EndTimestamp = DateTimeOffset.MinValue.ToUnixTimeMilliseconds(),
            User = user,
            IsGameRequest = isGame,
        });

        // Assert
        Assert.That(page.EventGroups.Count, Is.GreaterThan(0));
        Assert.That(page.EventGroups[0].Children.Count, Is.GreaterThan(0));
        Assert.That(page.EventGroups[0].Children[0].Events.Count, Is.EqualTo(isGame ? 2 : 4));

        // There should be only 1 user in the page
        Assert.That(page.Users.Count, Is.EqualTo(1));
    }

    [Test]
    public void ModerationEventHiddenFromIrrelevantUser()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameUser mod = context.CreateUser();
        GameUser moron = context.CreateUser();
        GameLevel level = context.CreateLevel(user);

        Event ev = context.Database.CreateEvent(level, new()
        {
            Actor = mod,
            EventType = EventType.LevelUnpublish,
            OverType = EventOverType.Moderation,
        });

        context.Database.Refresh();

        DatabaseActivityPage page = context.Database.GetGlobalRecentActivityPage(new()
        {
            Timestamp = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds(),
            EndTimestamp = DateTimeOffset.MinValue.ToUnixTimeMilliseconds(),
            User = moron,
        });

        // Assert
        Assert.That(page.EventGroups.Count, Is.Zero);
        Assert.That(page.Levels.Count, Is.Zero);
        Assert.That(page.Users.Count, Is.Zero);
    }

    [Test]
    public void ModerationEventShownToInvolvedUser()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameUser mod = context.CreateUser();
        GameLevel level = context.CreateLevel(user);

        Event ev = context.Database.CreateEvent(level, new()
        {
            Actor = mod,
            EventType = EventType.LevelUnpublish,
            OverType = EventOverType.Moderation,
        });

        context.Database.Refresh();

        DatabaseActivityPage page = context.Database.GetGlobalRecentActivityPage(new()
        {
            Timestamp = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds(),
            EndTimestamp = DateTimeOffset.MinValue.ToUnixTimeMilliseconds(),
            User = user,
        });

        // Assert
        Assert.That(page.EventGroups.Count, Is.EqualTo(1));
        Assert.That(page.Levels.Count, Is.EqualTo(1));
        Assert.That(page.Users.Count, Is.EqualTo(2));
    }

    [Test]
    public void ModerationEventShownToActor()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameUser mod = context.CreateUser();
        GameLevel level = context.CreateLevel(user);

        Event ev = context.Database.CreateEvent(level, new()
        {
            Actor = mod,
            EventType = EventType.LevelUnpublish,
            OverType = EventOverType.Moderation,
        });

        context.Database.Refresh();

        DatabaseActivityPage page = context.Database.GetGlobalRecentActivityPage(new()
        {
            Timestamp = DateTimeOffset.MaxValue.ToUnixTimeMilliseconds(),
            EndTimestamp = DateTimeOffset.MinValue.ToUnixTimeMilliseconds(),
            User = mod,
        });

        // Assert
        Assert.That(page.EventGroups.Count, Is.EqualTo(1));
        Assert.That(page.Levels.Count, Is.EqualTo(1));
        Assert.That(page.Users.Count, Is.EqualTo(2));
    }
}