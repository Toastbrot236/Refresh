using System.Xml.Serialization;
using Refresh.GameServer.Types.Levels;

namespace Refresh.GameServer.Types.Lists.Results;

[XmlRoot("results")]
[XmlType("results")]
public class SerializedMinimalLevelResultsList : SerializedMinimalLevelList, ISerializedCategoryItemResultsList
{
    public SerializedMinimalLevelResultsList() {}
    
    public SerializedMinimalLevelResultsList(IEnumerable<GameMinimalLevelResponse>? list, int total, int skip)
    {
        this.Total = total;
        this.Items = list?.ToList() ?? [];
        this.NextPageStart = skip + 1;
    }
}