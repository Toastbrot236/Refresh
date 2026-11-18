namespace Refresh.Database.Models.Assets.CWLib;

public partial class FarcHashesData : ISequentialId
{
    [Key] public int Version { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
    public int GeneratedUsingJobVersion { get; set; }
    public string DlcHeaderHash { get; set; } = "0";
    public string PinFarcHash { get; set; } = "0";
    public string PackFarcHash { get; set; } = "0";
    public string PinResourceHash { get; set; } = "0";
    public string PackResourceHash { get; set; } = "0";
    public string MapHash { get; set; } = "0";
    public string EnglishTransTableHash { get; set; } = "0";
    
    [NotMapped] public int SequentialId
    {
        get => this.Version;
        set => this.Version = value;
    }
}