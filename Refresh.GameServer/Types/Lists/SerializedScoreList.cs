using System.Xml.Serialization;
using Refresh.GameServer.Types.UserData.Leaderboard;

namespace Refresh.GameServer.Types.Lists;

[XmlRoot("scores")]
public class SerializedScoreList
{
    [XmlElement("playRecord")]
    public List<SerializedLeaderboardScore> Scores { get; set; } = new();
    [XmlAttribute("totalNumScores")]
    public int TotalScoreCount { get; set; }
    [XmlAttribute("yourScore")]
    public string? YourScore { get; set; }
    [XmlAttribute("yourRank")]
    public string? YourRank { get; set; }

    public static SerializedScoreList FromSubmittedEnumerable(IEnumerable<GameSubmittedScore> list, int skip, int? totalScoreCount = null, int? yourScore = null, int? yourRank = null)
    {
        SerializedScoreList value = new();
        int i = skip;
        foreach (GameSubmittedScore score in list)
        {
            i++;

            value.Scores.Add(new SerializedLeaderboardScore
            {
                Player = score.Players.FirstOrDefault()?.Username ?? "",
                Score = score.Score,
                Rank = i,
            });
        }

        // Take given total score count, else take the amount of scores given
        if (totalScoreCount != null) value.TotalScoreCount = (int)totalScoreCount;
        else value.TotalScoreCount = list.Count();

        // Nullable integers cannot be serialized apparently, so serialize these values as nullable strings instead,
        // to be able to exclude them from the response if not needed
        if (yourScore != null) value.YourScore = yourScore + "";
        if (yourRank != null) value.YourRank = yourRank + "";
        
        return value;
    }
}