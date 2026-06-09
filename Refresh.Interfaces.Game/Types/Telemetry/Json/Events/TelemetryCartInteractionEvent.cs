namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

// This data type encompasses the AddToCart, RemoveFromCart, and ItemDetailView events
[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryCartInteractionEvent
{
    public string StoreSessionId { get; set; } = "";
    public string Sku { get; set; } = "";
}