namespace Refresh.Core.Helpers;

public abstract class StringHelper
{
    public static string GetRefreshUserAgent(string instanceName, string webExternalUrl, string instanceContactData, string version)
        => $"{instanceName} Refresh.GameServer/v{version} (+{webExternalUrl}; contact: {instanceContactData})";
}