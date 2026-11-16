using System.Net;
using Bunkum.Core.Responses;

namespace Refresh.Interfaces.APIv4.Types.Api;

/// <summary>
/// A response from the API.
/// </summary>
[JsonObject(MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiResponse<TData> : IHasResponseCode where TData : class
{
    /// <summary>
    /// Empty constructor for serialization. Do not use.
    /// </summary>
    [Obsolete("Empty constructor for serialization.", true)]
    public ApiResponse()
    {
        
    }
    
    [JsonProperty("status")] public HttpStatusCode StatusCode { get; private set; }
    
    [JsonProperty("success")] public bool Success { get; private init; }
    [JsonProperty("data")] public TData? Data { get; private init; }

    /// <summary>
    /// Contains errors, warnings, and potentially also success messages.
    /// </summary>
    [JsonProperty("messages")] public List<ApiMessage> Messages { get; private init; } = [];
}