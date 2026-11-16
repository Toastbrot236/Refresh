using Newtonsoft.Json.Converters;

namespace Refresh.Interfaces.APIv4.Types.Api;

// From now on, we will always serialize our enum values as strings.
// This way, clients don't have to copy the entire enums with all values; if there is an unrecognized
// value, they can just show this string representation, which will usually be far more user-friendly
// than just showing a number. Even LBP itself does it like this (usually).
[JsonConverter(typeof(StringEnumConverter))]
public enum ApiMessageType
{
    /// <summary>
    /// Something has gone wrong that caused the request to fail.
    /// </summary>
    Error,

    /// <summary>
    /// Something not very important has gone wrong. The request wasn't cancelled because of it,
    /// but you want to notify the client user of this failure anyway.
    /// </summary>
    Warning,

    /// <summary>
    /// An operation has succeeded. Usually not very necessary.
    /// </summary>
    Success,
}