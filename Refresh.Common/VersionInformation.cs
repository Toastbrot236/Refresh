using System.Reflection;

namespace Refresh.Common;

public static class VersionInformation
{
    public static readonly string Version;

    static VersionInformation()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        AssemblyInformationalVersionAttribute? versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        string[] versionParts = versionAttribute?.InformationalVersion.Split('+') ?? [];
        string semanticVersion = versionParts.Length >= 1 ? versionParts[0] : "";
        string commitHash = versionParts.Length >= 2 ? versionParts[1] : "";

        // dotnet sets it to 1.0.0 if we haven't specified anything. We only specify via release tags, in which case a Github workflow
        // (release.yml) will take care of injecting the correct version while building Refresh, so builds made outside of that
        // (e.g. debug builds) will not have it explicitly set. We're long past v1.0.0,
        // so it's safe to unset the semantic version if it's 1.0.0, since then we know it wasn't set during building.
        if (semanticVersion is "0.0.0" or "1.0.0") semanticVersion = "";

        if (string.IsNullOrWhiteSpace(semanticVersion) && string.IsNullOrWhiteSpace(commitHash))
            Version = "unknown";

        else if (string.IsNullOrWhiteSpace(semanticVersion))
            Version = commitHash; // just the hash if semantic isn't set

        else if (string.IsNullOrWhiteSpace(commitHash))
            Version = semanticVersion; // just the semantic if hash is somehow not set (still have to find a case in which this would happen)

        else
            Version = $"{semanticVersion}+{commitHash}"; // original formatting
    }
}