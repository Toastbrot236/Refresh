using System.Reflection;
using Bunkum.Core;
using Bunkum.Core.Services;
using Bunkum.Core.Storage;
using Bunkum.Protocols.Http;
using JetBrains.Annotations;
using NotEnoughLogs;
using NotEnoughLogs.Behaviour;
using NotEnoughLogs.Sinks;
using Refresh.Common.Time;
using Refresh.Core.Configuration;
using Refresh.Core.Services;
using Refresh.Core.Types.Categories;
using Refresh.Core.Types.Data;
using Refresh.GameServer;
using Refresh.Database;
using RefreshTests.GameServer.Logging;
using RefreshTests.GameServer.Time;

namespace RefreshTests.GameServer.GameServer;

public class TestRefreshGameServer : RefreshGameServer
{
    public TestRefreshGameServer(BunkumHttpListener listener, Func<GameDatabaseProvider> provider, IDataStore? dataStore = null) : base(listener, provider, null, dataStore ?? new InMemoryDataStore())
    {}

    protected override ConfigStore CreateConfigStore()
    {
        return new ConfigStore();
    }

    public GameServerConfig GameServerConfig => this._configStore.GameServer;

    public override void Start()
    {
        this.Server.Start(0);
        // this.WorkerManager.Start();
    }

    public IDateTimeProvider DateTimeProvider { get; set; } = new MockDateTimeProvider();
    
    protected override IDateTimeProvider GetTimeProvider() => this.DateTimeProvider;

    protected override (LoggerConfiguration logConfig, List<ILoggerSink>? sinks) GetLoggerConfiguration()
    {
        LoggerConfiguration logConfig = new()
        {
            Behaviour = new DirectLoggingBehaviour(),
            MaxLevel = LogLevel.Trace,
        };

        List<ILoggerSink> sinks = new(1)
        {
            new NUnitSink(),
        };
        
        return (logConfig, sinks);
    }

    protected override void SetupMiddlewares()
    {
        
    }

    protected override void SetupServices()
    {
        this.Server.AddService<TimeProviderService>(this.DateTimeProvider);
        this.Server.AddService<CategoryService>();
        this.Server.AddService<MatchService>();
        this.Server.AddService<ImportService>();
        this.Server.AddService(new PresenceService(this.Logger, this._configStore.Integration!));
        this.Server.AddService<PlayNowService>();
        this.Server.AddService<CommandService>();
        this.Server.AddService<GuidCheckerService>();
        this.Server.AddService<SmtpService>();
        
        // Must always be last, see comment in RefreshGameServer
        this.Server.AddService<DataContextService>();
    }
    
    [Pure]
    public TService GetService<TService>() where TService : Service
    {
        List<Service> services = (List<Service>)typeof(BunkumServer).GetField("_services", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(this.Server)!;

        return (TService)services.First(s => typeof(TService) == s.GetType());
    }
}