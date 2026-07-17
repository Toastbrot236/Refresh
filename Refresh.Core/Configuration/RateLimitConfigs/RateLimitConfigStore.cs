using Bunkum.Core;
using Bunkum.Core.Configuration;
using NotEnoughLogs;

namespace Refresh.Core.Configuration.RateLimitConfigs;

public class RateLimitConfigStore
{
    public const string SubDirectoryPath = "endpointRateLimits";
    public EndpointRateLimitConfig MiscEndpointLimits { get; }
    public EndpointRateLimitConfig InstanceEndpointLimits { get; }
    public EndpointRateLimitConfig AuthEndpointLimits { get; }
    public EndpointRateLimitConfig ActivityEndpointLimits { get; }
    public EndpointRateLimitConfig NotificationEndpointLimits { get; }
    public EndpointRateLimitConfig LevelsEndpointLimits { get; }
    public EndpointRateLimitConfig LevelScoresEndpointLimits { get; }
    public EndpointRateLimitConfig UsersEndpointLimits { get; }
    public EndpointRateLimitConfig CategoriesEndpointLimits { get; }
    public EndpointRateLimitConfig PlaylistsEndpointLimits { get; }
    public EndpointRateLimitConfig ChallengesEndpointLimits { get; }
    public EndpointRateLimitConfig ChallengeScoresEndpointLimits { get; }
    public EndpointRateLimitConfig PhotosEndpointLimits { get; }
    public EndpointRateLimitConfig CommentsEndpointLimits { get; }
    public EndpointRateLimitConfig ReviewsEndpointLimits { get; }
    public EndpointRateLimitConfig ContestsEndpointLimits { get; }

    private static readonly Lock ConfigLock = new();
    public RateLimitConfigStore(Logger logger)
    {
        lock (ConfigLock)
        {
            this.MiscEndpointLimits                = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/miscellaneous.json", logger);
            this.InstanceEndpointLimits            = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/instance.json", logger);
            this.AuthEndpointLimits                = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/authentication.json", logger);
            this.ActivityEndpointLimits            = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/activity.json", logger);
            this.NotificationEndpointLimits        = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/notifications.json", logger);
            this.LevelsEndpointLimits              = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/levels.json", logger);
            this.LevelScoresEndpointLimits         = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/levelScores.json", logger);
            this.UsersEndpointLimits               = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/users.json", logger);
            this.CategoriesEndpointLimits          = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/categories.json", logger);
            this.PlaylistsEndpointLimits           = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/playlists.json", logger);
            this.ChallengesEndpointLimits          = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/challenges.json", logger);
            this.ChallengeScoresEndpointLimits     = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/challengeScores.json", logger);
            this.PhotosEndpointLimits              = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/photos.json", logger);
            this.CommentsEndpointLimits            = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/comments.json", logger);
            this.ReviewsEndpointLimits             = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/reviews.json", logger);
            this.ContestsEndpointLimits            = Config.LoadFromJsonFile<EndpointRateLimitConfig>($"{SubDirectoryPath}/contests.json", logger);
        }
    }

    public RateLimitConfigStore()
    {
        this.MiscEndpointLimits = new EndpointRateLimitConfig();
        this.InstanceEndpointLimits = new EndpointRateLimitConfig();
        this.AuthEndpointLimits = new EndpointRateLimitConfig();
        this.ActivityEndpointLimits = new EndpointRateLimitConfig();
        this.NotificationEndpointLimits = new EndpointRateLimitConfig();
        this.LevelsEndpointLimits = new EndpointRateLimitConfig();
        this.LevelScoresEndpointLimits = new EndpointRateLimitConfig();
        this.UsersEndpointLimits = new EndpointRateLimitConfig();
        this.CategoriesEndpointLimits = new EndpointRateLimitConfig();
        this.PlaylistsEndpointLimits = new EndpointRateLimitConfig();
        this.ChallengesEndpointLimits = new EndpointRateLimitConfig();
        this.ChallengeScoresEndpointLimits = new EndpointRateLimitConfig();
        this.PhotosEndpointLimits = new EndpointRateLimitConfig();
        this.CommentsEndpointLimits = new EndpointRateLimitConfig();
        this.ReviewsEndpointLimits = new EndpointRateLimitConfig();
        this.ContestsEndpointLimits
            = new EndpointRateLimitConfig();
    }

    public void AddToBunkum(BunkumServer server)
    {
        server.AddConfig(this.MiscEndpointLimits);
        server.AddConfig(this.InstanceEndpointLimits);
        server.AddConfig(this.AuthEndpointLimits);
        server.AddConfig(this.ActivityEndpointLimits);
        server.AddConfig(this.NotificationEndpointLimits);
        server.AddConfig(this.LevelsEndpointLimits);
        server.AddConfig(this.LevelScoresEndpointLimits);
        server.AddConfig(this.UsersEndpointLimits);
        server.AddConfig(this.CategoriesEndpointLimits);
        server.AddConfig(this.PlaylistsEndpointLimits);
        server.AddConfig(this.ChallengesEndpointLimits);
        server.AddConfig(this.ChallengeScoresEndpointLimits);
        server.AddConfig(this.PhotosEndpointLimits);
        server.AddConfig(this.CommentsEndpointLimits);
        server.AddConfig(this.ReviewsEndpointLimits);
        server.AddConfig(this.ContestsEndpointLimits);
    }
}