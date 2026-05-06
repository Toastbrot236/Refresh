using System.Text;
using Bunkum.Core;
using Bunkum.Core.Services;
using Bunkum.Listener.Protocol;
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

    public BlueSphereClientService(Logger logger, GameServerConfig config, IntegrationConfig integration, ContactInfoConfig contact) : base(logger)
    {
        if (!integration.BlueSphereEnabled)
        {
            throw new InvalidOperationException($"Cannot construct BlueSphereClientService because integration is disabled. This should be caught beforehand!");
        }

        this._client = new HttpClient
        {
            BaseAddress = new Uri(integration.BlueSphereBaseUrl),
        };
        this._client.DefaultRequestHeaders.Add("User-Agent", StringHelper.GetRefreshUserAgent(config.InstanceName, config.WebExternalUrl, contact.EmailAddress, VersionInformation.Version));
        this._client.DefaultRequestHeaders.Add("Content-Type", ContentType.Json);
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

        string serializedRequest = JsonConvert.SerializeObject(request);
        this.Logger.LogDebug(BunkumCategory.Authentication, $"Sending verification request to BlueSphere for username '{username}', code '{code}': '{serializedRequest}'");

        HttpResponseMessage message = await this._client.PostAsync("/api/verify", new StringContent(serializedRequest));

        string serializedResponse = Encoding.UTF8.GetString(await message.Content.ReadAsByteArrayAsync());
        this.Logger.LogDebug(BunkumCategory.Authentication, $"Received BlueSphere verification response for username '{username}', code '{code}': '{serializedResponse}'");

        if (!message.IsSuccessStatusCode)
        {
            // TODO: deduplicate JSON serialization and similar stuff
            BsErrorResponse? error = JsonConvert.DeserializeObject<BsErrorResponse>(serializedResponse);
            if (error == null) throw new BsOAuthException($"Couldn't deserialize error response for {code}! (status: {message.StatusCode})");

            throw new BsOAuthException($"Authentication failed for code '{code}' (status: {message.StatusCode}): '{error.Error}'");
        }

        BsOAuthResponse? success = JsonConvert.DeserializeObject<BsOAuthResponse>(serializedResponse);
        if (success == null) throw new BsOAuthException($"Couldn't deserialize success response for code '{code}'!");

        return success;
    }
}