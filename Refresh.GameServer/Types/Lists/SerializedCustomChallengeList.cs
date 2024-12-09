using System.Xml.Serialization;
using Refresh.GameServer.Types.Challenges.LbpHub;

namespace Refresh.GameServer.Types.Lists;

[XmlRoot("challenge")]
[XmlType("challenges")]
public class SerializedCustomChallengeList
{
    public SerializedCustomChallengeList() {}

    public SerializedCustomChallengeList(IEnumerable<SerializedCustomChallenge> items)
    {
        this.Items = items.ToList();
    }
    
    [XmlElement("challenge")]
    public List<SerializedCustomChallenge> Items { get; set; } = new();
}