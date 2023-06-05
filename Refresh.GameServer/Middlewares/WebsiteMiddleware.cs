using Bunkum.CustomHttpListener.Request;
using Bunkum.HttpServer;
using Bunkum.HttpServer.Database;
using Bunkum.HttpServer.Endpoints.Middlewares;

namespace Refresh.GameServer.Middlewares;

public class WebsiteMiddleware : IMiddleware
{
    private static readonly string WebPath = Path.Join(BunkumFileSystem.DataDirectory, "web");
    private static readonly Dictionary<string, string> MimeMapping = new()
    {
        { ".html", "text/html" },
        { ".css", "text/css" },
        { ".js", "application/javascript" },
        { ".svg", "image/svg+xml" },
        { ".ico", "image/vnd.microsoft.icon" },
    };

    private static bool HandleWebsiteRequest(ListenerContext context)
    {
        if (!Directory.Exists(WebPath)) // If website is not included in this build
            return false;

        string uri = context.Uri.AbsolutePath;

        if (uri.StartsWith("/lbp") || uri.StartsWith("/api") || uri.StartsWith("/autodiscover") || uri == "/_health") return false;

        if (uri == "/" || (context.RequestHeaders["Accept"] ?? "").Contains("text/html"))
            uri = "/index.html";

        string path = Path.GetFullPath(Path.Join(WebPath, uri));
        if (!path.StartsWith(WebPath)) return false; // check if path is within WebPath, prevents path traversal
        
        if (!File.Exists(path)) return false;

        string ext = Path.GetExtension(uri);
        string mime = MimeMapping.GetValueOrDefault(ext, "application/octet-stream");
        
        context.ResponseStream.Position = 0;
        context.ResponseCode = OK;
        context.ResponseHeaders["Content-Type"] = mime;
        context.ResponseHeaders["Cache-Control"] = "max-age=43200";
        
        context.Write(File.ReadAllBytes(path));
        context.FlushResponseAndClose();
        return true;
    }
    
    public void HandleRequest(ListenerContext context, Lazy<IDatabaseContext> database, Action next)
    {
        if (!HandleWebsiteRequest(context)) next();
    }
}