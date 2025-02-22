using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.Game.DataTypes.Response;

namespace Refresh.GameServer.Types.Lists;

#nullable disable

[XmlRoot("users")]
public class SerializedUserList : SerializedList<GameUserResponse>
{
    public SerializedUserList() {}
    
    public SerializedUserList(IEnumerable<GameUserResponse> list, int total, int skip)
    {
        this.Total = total;
        this.Items = list.ToList();
        this.NextPageStart = skip + 1;
    }

    [XmlElement("user")]
    public override List<GameUserResponse> Items { get; set; }
}