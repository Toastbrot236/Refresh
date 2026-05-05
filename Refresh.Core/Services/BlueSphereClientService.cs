using System.Text;
using Bunkum.Core.Services;
using NotEnoughLogs;
using Refresh.Core.Configuration;
using Refresh.Core.Helpers;
using Refresh.Core.Types.BlueSphere;

namespace Refresh.Core.Services;

/// <summary>
/// Manages OAuth requests to BlueSphere
/// </summary>
public class BlueSphereClientService : EndpointService
{
    private readonly HttpClient _client;
    private readonly GameServerConfig _config;
    private readonly IntegrationConfig _integration;
    private readonly ContactInfoConfig _contact;
    private readonly TimeProviderService _timeProviderService;
    public readonly string _refreshUserAgent;

    public BlueSphereClientService(Logger logger, GameServerConfig config, IntegrationConfig integration, ContactInfoConfig contact, TimeProviderService timeProviderService) : base(logger)
    {
        if (!integration.BlueSphereEnabled)
        {
            throw new InvalidOperationException($"Cannot construct BlueSphereClientService because integration is disabled. This should be caught beforehand!");
        }

        this._config = config;
        this._integration = integration;
        this._contact = contact;
        this._timeProviderService = timeProviderService;

        this._client = new HttpClient
        {
            BaseAddress = new Uri(integration.BlueSphereBaseUrl),
        };

        // cache it
        this._refreshUserAgent = StringHelper.GetRefreshUserAgent(config.InstanceName, config.WebExternalUrl, contact.EmailAddress, VersionInformation.Version);
    }

    /// <summary>
    /// Handles sending a request with the given code to the configured URL with the (partially) configured user agent, and tries to deserialize the response.
    /// </summary>
    public async Task<BsOAuthResponse> VerifyOAuthCode(string username, string code)
    {
        BsOAuthRequest request = new()
        {
            AuthCode = code,
        };
        HttpResponseMessage message = await this._client.PostAsync("/api/verify", new StringContent(JsonConvert.SerializeObject(request)));

        if (!message.IsSuccessStatusCode)
        {
            // TODO: deduplicate JSON serialization and similar stuff
            BsErrorResponse? error = JsonConvert.DeserializeObject<BsErrorResponse>(Encoding.UTF8.GetString(await message.Content.ReadAsByteArrayAsync()));
            if (error == null) throw new BsOAuthException($"Couldn't deserialize error response for {code}! (status: {message.StatusCode})");

            throw new BsOAuthException($"Authentication failed for {code} (status: {message.StatusCode}): '{error.Error}'");
        }

        BsOAuthResponse? success = JsonConvert.DeserializeObject<BsOAuthResponse>(Encoding.UTF8.GetString(await message.Content.ReadAsByteArrayAsync()));
        if (success == null) throw new BsOAuthException($"Couldn't deserialize success response for {code}!");

        return success;
    }
}