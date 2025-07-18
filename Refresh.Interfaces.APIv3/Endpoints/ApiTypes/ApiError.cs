using System.Net;
using Bunkum.Core.Responses;
using Bunkum.Listener.Protocol;

namespace Refresh.Interfaces.APIv3.Endpoints.ApiTypes;

/// <summary>
/// An error indicating a problem with the server, your query, or anything else.
/// </summary>
[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiError
{
    public ApiError(string message, HttpStatusCode code = BadRequest)
    {
        this.Name = this.GetType().Name;
        this.Message = message;
        this.StatusCode = code;
    }
    
    /// <summary>
    /// The name of the error.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A description of the error.
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// A numerical status code of the error.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    public static implicit operator Response(ApiError error) 
        => new(error, ContentType.Json, error.StatusCode);
}