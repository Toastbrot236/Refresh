using System.Xml.Serialization;
using Refresh.Interfaces.Game.Endpoints.DataTypes.Response;
using Refresh.Interfaces.Game.Types.Levels;

namespace Refresh.Interfaces.Game.Types.Lists;

#nullable disable

[XmlRoot("slots")]
[XmlType("slots")]
public class SerializedMinimalLevelList : SerializedList<GameMinimalLevelResponse>
{
    public SerializedMinimalLevelList() {}
    
    #nullable restore
    public SerializedMinimalLevelList(IEnumerable<GameMinimalLevelResponse> list, int total, int skip, IEnumerable<GameUserResponse>? users = null)
    {
        this.Total = total;
        this.Items = list.ToList();
        this.NextPageStart = skip + 1;
        this.Users = users?.ToList() ?? [];
    }

    [XmlElement("slot")]
    public override List<GameMinimalLevelResponse> Items { get; set; }

    [XmlElement("user")]
    public List<GameUserResponse> Users { get; set; }
}