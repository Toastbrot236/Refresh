using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryInventoryClickType
{
    PrimMat,
    ReadyMade,
    Decor,
    Sticker,
    CostumeMat,
    Joint,
    UsrObj,
    Bckgrnd,
    GameplayKit,
    UsrSticker,
    PrimShape,
    Danger,
    EyetoySticker,
    Gadget,
    Tool,
    SackbotMesh,
    PlayerCol,
    UsrCostume,
    Music,
    Sound,
    Instrument,
    Creatures,
}