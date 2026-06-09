using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryResourceErrorType
{
    /// <summary>
    /// Used as a generic "unknown error" case
    /// </summary>
    [EnumMember(Value = "REFLECT_ERROR")]
    ReflectError,
    [EnumMember(Value = "NoErrorState")]
    NoErrorState,
    [EnumMember(Value = "LOAD_STATE_UNLOADED")]
    Unloaded,
    [EnumMember(Value = "LOAD_STATE_LOADING_DATA")]
    LoadingData,
    [EnumMember(Value = "LOAD_STATE_PENDING_DESERIALISE")]
    PendingDeserialise,
    [EnumMember(Value = "LOAD_STATE_DESERIALISING")]
    Deserialising,
    [EnumMember(Value = "LOAD_STATE_LOADED")]
    Loaded,
    [EnumMember(Value = "LOAD_STATE_LOADED_SERIALISING")]
    LoadedSerialising,
    [EnumMember(Value = "LOAD_STATE_ERROR")]
    Error,
    [EnumMember(Value = "LOAD_STATE_ERROR_BADHASH")]
    BadHash,
    [EnumMember(Value = "LOAD_STATE_ERROR_FILENOTFOUND")]
    FileNotFound,
    [EnumMember(Value = "LOAD_STATE_ERROR_NO_DATA_SOURCE")]
    NoDataSource,
    [EnumMember(Value = "LOAD_STATE_ERROR_MODERATED")]
    Moderated,
}