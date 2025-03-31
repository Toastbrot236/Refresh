using Realms;
using Refresh.GameServer.Authentication;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Pins;

#nullable disable

public partial class UserProfilePinRelation : IRealmObject
{
    public UserPinProgressRelation pin { get; set; }
    public DateTimeOffset AddedToProfileTimestamp { get; set; }
    public TokenGame GameToShowIn
    {
        get => (TokenGame)this._GameToShowIn;
        set => this._GameToShowIn = (int)value;
    }
    internal int _GameToShowIn { get; set; }
}