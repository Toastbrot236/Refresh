namespace Refresh.Interfaces.Game.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryHardwareProfileEvent
{
    public string OpenPsid { get; set; } = ""; // in hex, seemingly?
    public int LanguageSetting { get; set; }
    public string TvResolution { get; set; } = ""; // in the format of `%dx%d`    
    public bool Capable3d { get; set; } 
    public int RefreshRate { get; set; } // possible values: 50, 60
    public TelemetryHardwareProfileHeadsetType HeadsetType { get; set; }
    public TelemetryHardwareProfileCameraType CameraType { get; set; }
    public TelemetryHardwareProfileControllerType[] Controllers { get; set; } = [];
}