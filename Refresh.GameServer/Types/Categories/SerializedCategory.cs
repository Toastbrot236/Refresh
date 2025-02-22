using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Categories;

#nullable disable

[XmlType("category")]
public class SerializedCategory
{
    [XmlElement("name")] public string Name { get; set; }
    [XmlElement("description")] public string Description { get; set; }
    [XmlElement("url")] public string Url { get; set; }
    [XmlElement("tag")] public string Tag { get; set; }
    [XmlElement("icon")] public string IconHash { get; set; }
    
    [XmlArray("types")] 
    [XmlArrayItem("type")] public string[] Types { get; set; }

    public static SerializedCategory FromCategory(GameCategory category)
    {
        SerializedCategory serializedCategory = new()
        {
            Name = category.Name,
            Description = category.Description,
            Url = "/searches/" + category.ApiRoute,
            Tag = category.ApiRoute,
            IconHash = category.IconHash,
        };

        return serializedCategory;
    }
}