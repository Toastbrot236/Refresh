using System.Xml.Serialization;
using Refresh.GameServer.Types.Categories;
using Refresh.GameServer.Types.Categories.Levels;
using Refresh.GameServer.Types.Categories.Playlists;
using Refresh.GameServer.Types.Categories.Users;

namespace Refresh.GameServer.Types.Lists;

[XmlRoot("categories")]
[XmlType("categories")]
[XmlInclude(typeof(SerializedLevelCategory))]
[XmlInclude(typeof(SerializedPlaylistCategory))]
[XmlInclude(typeof(SerializedUserCategory))]
public class SerializedCategoryList : SerializedList<SerializedCategory>
{
    public SerializedCategoryList(IEnumerable<SerializedCategory> items, SearchLevelCategory searchCategory, int total)
    {
        this.Items = items.ToList();
        this.TextSearchCategory = SerializedCategory.FromCategory(searchCategory);
        this.Total = total;
    }

    public SerializedCategoryList() {}

    [XmlElement("category")]
    public sealed override List<SerializedCategory> Items { get; set; } = null!;

    [XmlElement("text_search")]
    public SerializedCategory TextSearchCategory { get; set; } = null!;

    [XmlAttribute("hint")]
    public string Hint { get; set; } = string.Empty;
}