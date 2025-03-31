using Realms;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Pins;

#nullable disable

public partial class UserPinProgressRelation : IRealmObject
{
    /// <summary>
    /// The identifier the game uses to refer to pins when it sends a SerializedUserPins request.
    /// The progress type is the same across different pins whose objective is the same, but in different quantities
    /// (for example, if a pin requires 2 aced levels and another one requires 5 aced levels but they are
    /// listed under the same pin in-game, they both use the same progress type)
    /// </summary>
    public long ProgressTypeId { get; set; }
    public int Progress { get; set; }
    public GameUser User { get; set; }
    public DateTimeOffset PublishDate { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; }

    /*
    public TokenGame GameVersion 
    {
        get => (TokenGame)this._GameVersion;
        set => this._GameVersion = (int)value;
    }
    
    // ReSharper disable once InconsistentNaming
    internal int _GameVersion { get; set; } 
    */
}