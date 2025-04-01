using Bunkum.Core.Configuration;
using Refresh.GameServer.Types.Pins;

namespace Refresh.GameServer.Configuration;

/// <summary>
/// Config which can be used to paste pin info from, for example, 
/// https://github.com/LittleBigRefresh/Docs/blob/main/Docs/pin-files/lbp3.json into.
/// </summary>
public class GamePinConfig : Config
{
    public override int CurrentConfigVersion => 1;
    public override int Version { get; set; } = 0;

    protected override void Migrate(int oldVer, dynamic oldConfig)
    {

    }

    public ImportedPin[] Lbp2Pins = [];
    public ImportedPin[] Lbp3Pins = [];
    public ImportedPin[] LbpVitaPins = [];
    public ImportedPin[] BetaBuildPins = [];
}