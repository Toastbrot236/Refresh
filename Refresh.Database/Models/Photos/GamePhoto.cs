using MongoDB.Bson;
using Refresh.Database.Models.Assets;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Users;

namespace Refresh.Database.Models.Photos;

#nullable disable

[JsonObject(MemberSerialization.OptOut)]
public partial class GamePhoto : ISequentialId
{
    [Key] public int PhotoId { get; set; }
    public DateTimeOffset TakenAt { get; set; }
    public DateTimeOffset PublishedAt { get; set; }
    
    [Required, ForeignKey(nameof(PublisherId))]
    public GameUser Publisher { get; set; }
    public ObjectId PublisherId { get; set; }

    #nullable restore
    [ForeignKey(nameof(LevelId))]
    public GameLevel? Level { get; set; }
    public int? LevelId { get; set; }
    #nullable disable

    public string LevelType { get; set; }
    public int OriginalLevelId { get; set; }
    public string OriginalLevelName { get; set; }
    
    [Required, ForeignKey(nameof(SmallAssetHash))]
    public GameAsset SmallAsset { get; set; }
    public string SmallAssetHash { get; set; }

    [Required, ForeignKey(nameof(MediumAssetHash))]
    public GameAsset MediumAsset { get; set; }
    public string MediumAssetHash { get; set; }

    [Required, ForeignKey(nameof(LargeAssetHash))]
    public GameAsset LargeAsset { get; set; }
    public string LargeAssetHash { get; set; }

    public string PlanHash { get; set; }

    #region Subjects
    
    [NotMapped]
    public IReadOnlyList<GamePhotoSubject> Subjects
    {
        get
        {
            List<GamePhotoSubject> subjects = new(4);
            
            if (this.Subject1DisplayName != null)
                subjects.Add(new GamePhotoSubject(this.Subject1User, this.Subject1DisplayName, this.Subject1Bounds));
            else return subjects;
            
            if (this.Subject2DisplayName != null)
                subjects.Add(new GamePhotoSubject(this.Subject2User, this.Subject2DisplayName, this.Subject2Bounds));
            else return subjects;
            
            if (this.Subject3DisplayName != null)
                subjects.Add(new GamePhotoSubject(this.Subject3User, this.Subject3DisplayName, this.Subject3Bounds));
            else return subjects;
            
            if (this.Subject4DisplayName != null)
                subjects.Add(new GamePhotoSubject(this.Subject4User, this.Subject4DisplayName, this.Subject4Bounds));

            return subjects;
        }
        set
        {
            if (value.Count > 4) throw new InvalidOperationException("Too many subjects. Should be caught beforehand by input validation");
            this.ClearSubjects();

            if (value.Count >= 1)
            {
                this.Subject1User = value[0].User;
                this.Subject1DisplayName = value[0].DisplayName;
                foreach (float bound in value[0].Bounds)
                    this.Subject1Bounds.Add(bound);
            }
            
            if (value.Count >= 2)
            {
                this.Subject2User = value[1].User;
                this.Subject2DisplayName = value[1].DisplayName;
                foreach (float bound in value[1].Bounds)
                    this.Subject2Bounds.Add(bound);
            }
            
            if (value.Count >= 3)
            {
                this.Subject3User = value[2].User;
                this.Subject3DisplayName = value[2].DisplayName;
                foreach (float bound in value[2].Bounds)
                    this.Subject3Bounds.Add(bound);
            }
            
            if (value.Count >= 4)
            {
                this.Subject4User = value[3].User;
                this.Subject4DisplayName = value[3].DisplayName;
                foreach (float bound in value[3].Bounds)
                    this.Subject4Bounds.Add(bound);
            }
        }
    }

    private void ClearSubjects()
    {
        this.Subject1User = null;
        this.Subject1DisplayName = null;
        this.Subject1Bounds.Clear();
        
        this.Subject2User = null;
        this.Subject2DisplayName = null;
        this.Subject2Bounds.Clear();
        
        this.Subject3User = null;
        this.Subject3DisplayName = null;
        this.Subject3Bounds.Clear();
        
        this.Subject4User = null;
        this.Subject4DisplayName = null;
        this.Subject4Bounds.Clear();
    }

#nullable enable
    #pragma warning disable CS8618 // realm forces us to have a non-nullable IList<float> so we have to have these shenanigans
    
    public ObjectId? Subject1UserId { get; set; }
    [ForeignKey(nameof(Subject1UserId))]
    public GameUser? Subject1User { get; set; }
    public string? Subject1DisplayName { get; set; }
    
    public ObjectId? Subject2UserId { get; set; }
    [ForeignKey(nameof(Subject2UserId))]
    public GameUser? Subject2User { get; set; }
    public string? Subject2DisplayName { get; set; }
    
    public ObjectId? Subject3UserId { get; set; }
    [ForeignKey(nameof(Subject3UserId))]
    public GameUser? Subject3User { get; set; }
    public string? Subject3DisplayName { get; set; }
    
    public ObjectId? Subject4UserId { get; set; }
    [ForeignKey(nameof(Subject4UserId))]
    public GameUser? Subject4User { get; set; }
    public string? Subject4DisplayName { get; set; }
    
    public List<float> Subject1Bounds { get; set; } = [];
    public List<float> Subject2Bounds { get; set; } = [];
    public List<float> Subject3Bounds { get; set; } = [];
    public List<float> Subject4Bounds { get; set; } = [];
    
    #pragma warning restore CS8618
    #nullable disable
    
    #endregion
    
    [JsonIgnore] [NotMapped] public int SequentialId
    {
        get => this.PhotoId;
        set => this.PhotoId = value;
    }
}