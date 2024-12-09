using System.Xml.Serialization;
using Refresh.GameServer.Types.Challenges.LbpHub;

namespace Refresh.GameServer.Types.Lists;

[XmlRoot("challenge-scores")]
[XmlType("challenge-scores")]
public class SerializedCustomChallengeScoreboard
{
    public SerializedCustomChallengeScoreboard() {}

    public SerializedCustomChallengeScoreboard(IEnumerable<SerializedCustomChallengeScore> items)
    {
        this.Items = items.ToList();
    }
    
    [XmlElement("challenge-score")]
    public List<SerializedCustomChallengeScore> Items { get; set; } = [];
}