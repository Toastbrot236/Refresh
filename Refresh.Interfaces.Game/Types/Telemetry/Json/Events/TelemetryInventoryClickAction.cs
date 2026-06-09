using System.Collections.Frozen;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryInventoryClickAction // ps4 01.28 0x01ef1186
{
    Unknown,
    None,
    Create,
    CreatePrimitive,
    Fill,
    SetEmitterObject,
    SetGunObject,
    SetEggContents,
    AddCompleteReward,
    AddCollectReward,
    AddAceReward,
    SetStickerSwitchRef,
    SetShapeStickerCutter,
    SetLevelLink,
    RandomiseCostume,
    RandomiseNpcCostume,
    EditPainting,
    SetPaintingBackground,
    SetPaintingStickerBrush,
    PoppetPowerup,
    SetPocketItem,
    SetPocketSensor,
    SetPowerUpObject,
    SetPowerUpCostume,
    CreateGamekit,
    SetSackbotMesh,
    LevelImporter,
    SetMaterial,
}

public static class TelemetryInventoryClickActionExtensions
{
    public static FrozenDictionary<string, TelemetryInventoryClickAction> StringToValue = new Dictionary<string, TelemetryInventoryClickAction>
    {
        {"ACTION_NONE", TelemetryInventoryClickAction.None},
        {"ACTION_CREATE", TelemetryInventoryClickAction.Create},
        {"ACTION_CREATE_PRIMTIIVE", TelemetryInventoryClickAction.CreatePrimitive}, // typos intentional
        {"CREATE_PRIMTIIVE", TelemetryInventoryClickAction.CreatePrimitive},
        {"ACTION_FLOOD_FILL", TelemetryInventoryClickAction.Fill},
        {"ACTION_SET_EMITTER_OBJECT", TelemetryInventoryClickAction.SetEmitterObject},
        {"ACTION_SET_GUN_OBJECT", TelemetryInventoryClickAction.SetGunObject},
        {"ACTION_SET_EGG_CONTENTS", TelemetryInventoryClickAction.SetEggContents},
        {"ACTION_ADD_COMPLETE_REWARD", TelemetryInventoryClickAction.AddCompleteReward},
        {"ACTION_ADD_COLLECT_REWARD", TelemetryInventoryClickAction.AddCollectReward},
        {"ACTION_ADD_ACE_REWARD", TelemetryInventoryClickAction.AddAceReward},
        {"ACTION_SET_STICKER_SWITCH_REF", TelemetryInventoryClickAction.SetStickerSwitchRef},
        {"ACTION_SET_SHAPE_STICKER_CUTTER", TelemetryInventoryClickAction.SetShapeStickerCutter},
        {"ACTION_SET_LEVEL_LINK", TelemetryInventoryClickAction.SetLevelLink},
        {"ACTION_RANDOMISE_COSTUME", TelemetryInventoryClickAction.RandomiseCostume},
        {"ACTION_RANDOMISE_NPC_COSTUME", TelemetryInventoryClickAction.RandomiseNpcCostume},
        {"ACTION_EDIT_PAINTING", TelemetryInventoryClickAction.EditPainting},
        {"ACTION_SET_PAINTING_BACKGROUND", TelemetryInventoryClickAction.SetPaintingBackground},
        {"ACTION_SET_PAINTING_STICKER_BRUSH", TelemetryInventoryClickAction.SetPaintingStickerBrush},
        {"ACTION_POPPET_POWERUP", TelemetryInventoryClickAction.PoppetPowerup},
        {"ACTION_SET_POCKET_ITEM", TelemetryInventoryClickAction.SetPocketItem},
        {"ACTION_SET_POCKET_SENSOR", TelemetryInventoryClickAction.SetPocketSensor},
        {"ACTION_SET_POWER_UP_OBJECT", TelemetryInventoryClickAction.SetPowerUpObject},
        {"ACTION_SET_POWER_UP_COSTUME", TelemetryInventoryClickAction.SetPowerUpCostume},
        {"ACTION_CREATE_GAMEKIT", TelemetryInventoryClickAction.CreateGamekit},
        {"ACTION_LEVEL_IMPORTER", TelemetryInventoryClickAction.LevelImporter},
        {"ACTION_SET_SACKBOT_MESH", TelemetryInventoryClickAction.SetMaterial},
        {"ACTION_SET_MATERIAL", TelemetryInventoryClickAction.SetMaterial},
    }.ToFrozenDictionary();

    public static TelemetryInventoryClickAction FromGameString(string key)
        => StringToValue.GetValueOrDefault(key, TelemetryInventoryClickAction.Unknown);
}