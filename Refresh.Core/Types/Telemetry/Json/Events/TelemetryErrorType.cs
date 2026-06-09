using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Core.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryErrorType
{
    [EnumMember(Value = "GAME_DATA_BACKUP_CALLED_FAIL")]
    GameDataBackupCalledFail,
    [EnumMember(Value = "GAME_DATA_BACKUP_CALLED_COMPLETE")]
    GameDataBackupCalledComplete,
    [EnumMember(Value = "GAME_DATA_DELETED")]
    GameDataDeleted,
    [EnumMember(Value = "CORRUPT_PROFILE_MESSAGE_DISPLAYED_ADVENTURE")]
    CorruptProfileMessageDisplayedAdventure,
    [EnumMember(Value = "CORRUPT_PROFILE_MESSAGE_DISPLAYED_QUICK_LOAD")]
    CorruptProfileMessageDisplayedQuickLoad,
    [EnumMember(Value = "EGMT_SAVEGAME_INFO_PATCHED")]
    SavegameInfoPatched,
    [EnumMember(Value = "MEMORY_STOMP_DETECTED")]
    MemoryStompDetected,
    [EnumMember(Value = "BAD_DECORATION_DETECTED")]
    BadDecorationDetected,
    [EnumMember(Value = "EGMT_PLAYER_TEXT_MESSAGE")]
    PlayerTextMessage,
}