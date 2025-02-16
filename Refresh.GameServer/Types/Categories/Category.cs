namespace Refresh.GameServer.Types.Categories;

[JsonObject(MemberSerialization.OptIn)]
public abstract class Category
{
    [JsonProperty] public string Name { get; set; } = "";
    [JsonProperty] public string AlternativeName { get; set; } = "";
    [JsonProperty] public string Description { get; set; } = "";
    [JsonProperty] public string AlternativeDescription { get; set; } = "";
    [JsonProperty] public string IconHash { get; set; } = "0";
    [JsonProperty] public string FontAwesomeIcon { get; set; } = "faCertificate";
    [JsonProperty] public bool Hidden { get; set; } = false;

    [JsonProperty] public readonly bool RequiresUser;
    [JsonProperty] public readonly string ApiRoute;
    public readonly string[] GameRoutes;
    
    internal Category(string apiRoute, string gameRoute, bool requiresUser) : this(apiRoute, new []{gameRoute}, requiresUser) {}
    internal Category(string apiRoute, string[] gameRoutes, bool requiresUser)
    {
        this.ApiRoute = apiRoute;
        this.GameRoutes = gameRoutes;
        
        this.RequiresUser = requiresUser;
    }
}