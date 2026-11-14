using Bunkum.Core;
using Refresh.Common;
using Refresh.GameServer;

BunkumConsole.AllocateConsole();

#if DEBUG
Console.WriteLine($"Starting the Refresh Game server (DEBUG build, version {VersionInformation.Version})");
#else
Console.WriteLine($"Starting the Refresh Game server (RELEASE build, version {VersionInformation.Version})");
#endif

using RefreshGameServer server = new();

if (args.Length > 0)
{
    CommandLineManager cli = new(server);
    cli.StartWithArgs(args);
    
    return;
}

server.Start();
await Task.Delay(-1);