using Refresh.Database.Models.Assets;

namespace Refresh.Database.Models.Relations;

#nullable disable

[PrimaryKey(nameof(Dependent), nameof(Dependency))]
public partial class AssetDependencyRelation
{
    [Required]
    public string Dependent { get; set; }

    [Required, ForeignKey(nameof(Dependent))]
    public GameAsset DependentAsset { get; set; }

    [Required]
    public string Dependency { get; set; }
    
    [Required, ForeignKey(nameof(Dependency))]
    public GameAsset DependencyAsset { get; set; }
}