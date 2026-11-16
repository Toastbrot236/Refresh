using System.Net;
using AttribDoc;

namespace Refresh.Interfaces.APIv4.Types.Api;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiMessage : IApiResponse
{
    /// <summary>
    /// Title/name of error/warning/success message
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The content of the message
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Whether this message is an error, warning, or success
    /// </summary>
    public required string Type { get; set; } // success, warning, or error?
    
    public ApiMessage(string name, string message, ApiMessageType type)
    {
        this.Name = name;
        this.Message = message;
        this.Type = type.ToString();
    }
}