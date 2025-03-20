using Realms;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Types.Pins;
#nullable disable

public partial class PinUserRelation : IRealmObject
{
    public int PinId { get; set; }
    public int Progress { get; set; }
    public GameUser User { get; set; }
    public DateTimeOffset AchievedAt { get; set; }

    public bool Lbp2ProfilePin { get; set; }
    public bool Lbp3ProfilePin { get; set; }
    public bool LbpVitaProfilePin { get; set; }
    public bool BetaBuildProfilePin { get; set; }
}