using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryInventoryClickAction // ps4 01.28 0x01ef1186
{
    [EnumMember(Value = "ACTION_NONE")]
    None,
    [EnumMember(Value = "ACTION_CREATE")]
    Create,
    [EnumMember(Value = "ACTION_CREATE_PRIMTIIVE")] // typo is intentional
    CreatePrimitive,
    [EnumMember(Value = "ACTION_FLOOD_FILL")]
    Fill,
    [EnumMember(Value = "ACTION_SET_EMITTER_OBJECT")]
    SetEmitterObject,
    [EnumMember(Value = "ACTION_SET_GUN_OBJECT")]
    SetGunObject,
    [EnumMember(Value = "ACTION_SET_EGG_CONTENTS")]
    SetEggContents,
    [EnumMember(Value = "ACTION_ADD_COMPLETE_REWARD")]
    AddCompleteReward,
    [EnumMember(Value = "ACTION_ADD_COLLECT_REWARD")]
    AddCollectReward,
    [EnumMember(Value = "ACTION_ADD_ACE_REWARD")]
    AddAceReward,
    [EnumMember(Value = "ACTION_SET_STICKER_SWITCH_REF")]
    SetStickerSwitchRef,
    [EnumMember(Value = "ACTION_SET_SHAPE_STICKER_CUTTER")]
    SetShapeStickerCutter,
    [EnumMember(Value = "ACTION_SET_LEVEL_LINK")]
    SetLevelLink,
    [EnumMember(Value = "ACTION_RANDOMISE_COSTUME")]
    RandomiseCostume,
    [EnumMember(Value = "ACTION_RANDOMISE_NPC_COSTUME")]
    RandomiseNpcCostume,
    [EnumMember(Value = "ACTION_EDIT_PAINTING")]
    EditPainting,
    [EnumMember(Value = "ACTION_SET_PAINTING_BACKGROUND")]
    SetPaintingBackground,
    [EnumMember(Value = "ACTION_SET_PAINTING_STICKER_BRUSH")]
    SetPaintingStickerBrush,
    [EnumMember(Value = "ACTION_POPPET_POWERUP")]
    PoppetPowerup,
    [EnumMember(Value = "ACTION_SET_POCKET_ITEM")]
    SetPocketItem,
    [EnumMember(Value = "ACTION_SET_POCKET_SENSOR")]
    SetPocketSensor,
    [EnumMember(Value = "ACTION_SET_POWER_UP_OBJECT")]
    SotPowerUpObject,
    [EnumMember(Value = "ACTION_SET_POWER_UP_COSTUME")]
    SetPowerUpCostume,
    [EnumMember(Value = "ACTION_CREATE_GAMEKIT")]
    CreateGamekit,
    [EnumMember(Value = "ACTION_SET_SACKBOT_MESH")]
    SetSackbotMesh,
    [EnumMember(Value = "ACTION_LEVEL_IMPORTER")]
    LevelImporter,
    [EnumMember(Value = "ACTION_SET_MATERIAL")]
    SetMaterial,
}