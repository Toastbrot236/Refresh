namespace Refresh.GameServer.Endpoints.ApiV3.DataTypes.Response.Playlists;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiGamePlaylistRelationsResponse : IApiResponse
{
    public required bool IsHearted { get; set; }
}