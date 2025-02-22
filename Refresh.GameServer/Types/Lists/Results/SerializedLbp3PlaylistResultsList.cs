using System.Xml.Serialization;
using Refresh.GameServer.Types.Playlists;

namespace Refresh.GameServer.Types.Lists.Results;

[XmlRoot("results")]
[XmlType("results")]
public class SerializedLbp3PlaylistResultsList : SerializedLbp3PlaylistList, ISerializedCategoryResultsList
{
    public SerializedLbp3PlaylistResultsList() {}
    
    public SerializedLbp3PlaylistResultsList(IEnumerable<SerializedLbp3Playlist>? list, int total, int skip)
    {
        this.Total = total;
        this.Items = list?.ToList() ?? [];
        this.NextPageStart = skip + 1;
    }
}