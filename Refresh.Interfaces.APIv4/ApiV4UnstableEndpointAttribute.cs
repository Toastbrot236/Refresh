using Bunkum.Protocols.Http;
using JetBrains.Annotations;

namespace Refresh.Interfaces.APIv3;

[MeansImplicitUse]
public class ApiV4UnstableEndpointAttribute : HttpEndpointAttribute
{
    public const string BaseRoute = "/api/v4/unstable/";
    
    public string RouteWithParameters { get; }

    public ApiV4UnstableEndpointAttribute(string route, HttpMethods method = HttpMethods.Get, string contentType = Bunkum.Listener.Protocol.ContentType.Json)
        : base(BaseRoute + route, method, contentType)
    {
        this.RouteWithParameters = '/' + route;
    }

    public ApiV4UnstableEndpointAttribute(string route, string contentType, HttpMethods method = HttpMethods.Get)
        : this(route, method, contentType)
    {}
}