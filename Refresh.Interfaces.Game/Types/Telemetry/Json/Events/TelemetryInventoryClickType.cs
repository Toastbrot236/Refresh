using System.Collections.Frozen;
using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryInventoryClickType
{
    Unknown,
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

public static class TelemetryInventoryClickTypeExtensions
{
    public static FrozenDictionary<string, TelemetryInventoryClickType> StringToValue = new Dictionary<string, TelemetryInventoryClickType>
    {
        {"??", TelemetryInventoryClickType.Unknown},
        {"PrimMat", TelemetryInventoryClickType.PrimMat},
        {"ReadyMade", TelemetryInventoryClickType.ReadyMade},
        {"Decor", TelemetryInventoryClickType.Decor},
        {"Sticker", TelemetryInventoryClickType.Sticker},
        {"CostumeMat", TelemetryInventoryClickType.CostumeMat},
        {"Joint", TelemetryInventoryClickType.Joint},
        {"UsrObj", TelemetryInventoryClickType.UsrObj},
        {"Bckgrnd", TelemetryInventoryClickType.Bckgrnd},
        {"GameplayKit", TelemetryInventoryClickType.GameplayKit},
        {"UsrSticker", TelemetryInventoryClickType.UsrSticker},
        {"PrimShape", TelemetryInventoryClickType.PrimShape},
        {"Danger", TelemetryInventoryClickType.Danger},
        {"EyetoySticker", TelemetryInventoryClickType.EyetoySticker},
        {"Gadget", TelemetryInventoryClickType.Gadget},
        {"Tool", TelemetryInventoryClickType.Tool},
        {"SackbotMesh", TelemetryInventoryClickType.SackbotMesh},
        {"PlayerCol", TelemetryInventoryClickType.PlayerCol},
        {"UsrCostume", TelemetryInventoryClickType.UsrCostume},
        {"Music", TelemetryInventoryClickType.Music},
        {"Sound", TelemetryInventoryClickType.Sound},
        {"Instrument", TelemetryInventoryClickType.Instrument},
        {"Creatures", TelemetryInventoryClickType.Creatures},
    }.ToFrozenDictionary();

    public static TelemetryInventoryClickType FromGameString(string key)
        => StringToValue.GetValueOrDefault(key, TelemetryInventoryClickType.Unknown);
}