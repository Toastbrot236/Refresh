using System.Reflection;
using Bunkum.Core.Database;
using Bunkum.Core.RateLimit;
using Bunkum.Core.Responses;
using Bunkum.Core.Services;
using Bunkum.Listener.Protocol;
using Bunkum.Listener.Request;
using NotEnoughLogs;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Users;

namespace Refresh.Core.Services;

// Referenced from https://github.com/PlanetBunkum/Bunkum/blob/main/Bunkum.Core/Services/RateLimitService.cs
public class GameRateLimitService : Service
{
    private readonly IRateLimiter _rateLimiter;
    private readonly GameAuthenticationService _authService;

    internal GameRateLimitService(Logger logger, GameAuthenticationService authService, IRateLimiter rateLimiter) : base(logger)
    {
        this._rateLimiter = rateLimiter;
        this._authService = authService;
    }

    public override Response? OnRequestHandled(ListenerContext context, MethodInfo method, Lazy<IDatabaseContext> database)
    {
        Token? token = this._authService.AuthenticateToken(context, database);
        // Don't rely on user-agent as much so users couldn't just bypass the rate-limit by overwriting their user agent
        // TODO don't rely on PSP user agent to determine whether the game is PSP in other places either, for similar reasons
        bool isPsp = token?.TokenGame == TokenGame.LittleBigPlanetPSP;

        bool violated = false;

        if (token != null)
            violated = this._rateLimiter.UserViolatesRateLimit(context, method, token.User);
        else
            violated = this._rateLimiter.RemoteEndpointViolatesRateLimit(context, method);

        if (violated) return new Response("You have been rate-limited.", ContentType.Plaintext, TooManyRequests);
        return null;
    }
}