using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
public enum TelemetrySocialInteractionType
{
    MessageSent,
    Heart,
    FriendRequest,
    LeaveComment,
    GiftItem,
    PartyInvitation,
}