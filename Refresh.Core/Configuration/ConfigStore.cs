using Bunkum.Core;
using Bunkum.Core.Configuration;
using NotEnoughLogs;

namespace Refresh.Core.Configuration;

public class ConfigStore
{
    public GameServerConfig GameServer { get; }
    public DatabaseConfig Database { get; }
    
    public ContactInfoConfig ContactInfo { get; }
    public IntegrationConfig Integration { get; }
    public RichPresenceConfig RichPresence { get; }

    public DryArchiveConfig DryArchive { get; }
    public EndpointRateLimitConfig EndpointRateLimits { get; }
    public UserRoleConfig UserRoles { get; }

    private static readonly Lock ConfigLock = new();
    public ConfigStore(Logger logger)
    {
        lock (ConfigLock)
        {
            // Has to be loaded first so GameServerConfig could migrate its stuff over to it
            this.UserRoles = Config.LoadFromJsonFile<UserRoleConfig>("userRoleSettings.json", logger);

            this.GameServer = Config.LoadFromJsonFile<GameServerConfig>("refreshGameServer.json", logger, [this.UserRoles]);
            this.Database = Config.LoadFromJsonFile<DatabaseConfig>("db.json", logger);
        
            this.ContactInfo = Config.LoadFromJsonFile<ContactInfoConfig>("contactInfo.json", logger);
            this.Integration = Config.LoadFromJsonFile<IntegrationConfig>("integrations.json", logger);
            this.RichPresence = Config.LoadFromJsonFile<RichPresenceConfig>("rpc.json", logger);
        
            this.DryArchive = Config.LoadFromJsonFile<DryArchiveConfig>("dry.json", logger);
            this.EndpointRateLimits = Config.LoadFromJsonFile<EndpointRateLimitConfig>("endpointRateLimits.json", logger);
        }
    }

    public ConfigStore()
    {
        this.GameServer = new GameServerConfig();
        this.Database = new DatabaseConfig();

        this.ContactInfo = new ContactInfoConfig();
        this.Integration = new IntegrationConfig();
        this.RichPresence = new RichPresenceConfig();

        this.DryArchive = new DryArchiveConfig();
        this.EndpointRateLimits = new EndpointRateLimitConfig();
        this.UserRoles = new UserRoleConfig();
    }

    public void AddToBunkum(BunkumServer server)
    {
        server.AddConfig(this.GameServer);
        server.AddConfig(this.Database);
        server.AddConfig(this.ContactInfo);
        server.AddConfig(this.Integration);
        server.AddConfig(this.RichPresence);
        server.AddConfig(this.DryArchive);
        server.AddConfig(this.EndpointRateLimits);
        server.AddConfig(this.UserRoles);
    }
}