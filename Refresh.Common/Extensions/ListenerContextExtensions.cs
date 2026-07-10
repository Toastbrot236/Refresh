using Bunkum.Listener.Request;

namespace Refresh.Common.Extensions;

public static class ListenerContextExtensions
{
    public static bool IsPSP(this ListenerContext context) => context.RequestHeaders.Get("User-Agent") == "LBPPSP CLIENT";

    public static bool IsApi(this ListenerContext context) => context.Uri.AbsolutePath.StartsWith("/api/");
}