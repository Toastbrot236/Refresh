using System.Xml.Serialization;

namespace Refresh.Database.Models.Activity.Groups;

[XmlRoot("subgroups")]
[XmlType("subgroups")]
public class Subgroups
{
    [XmlElement("group")] public List<ActivityGroup> Items { get; set; } = new();

    public Subgroups(List<ActivityGroup> items)
    {
        this.Items = items;
    }

    public Subgroups() {}
}