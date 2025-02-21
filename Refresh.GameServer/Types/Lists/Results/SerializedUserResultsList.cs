using System.Xml.Serialization;
using Refresh.GameServer.Endpoints.Game.DataTypes.Response;

namespace Refresh.GameServer.Types.Lists.Results;

[XmlRoot("results")]
[XmlType("results")]
public class SerializedUserResultsList : SerializedUserList, ISerializedCategoryItemResultsList
{
    public SerializedUserResultsList() {}
    
    public SerializedUserResultsList(IEnumerable<GameUserResponse>? list, int total, int skip)
    {
        this.Total = total;
        this.Items = list?.ToList() ?? [];
        this.NextPageStart = skip + 1;
    }
}