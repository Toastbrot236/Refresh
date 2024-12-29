using System.Xml.Serialization;
using Refresh.Common.Constants;
using Refresh.GameServer.Endpoints.ApiV3.DataTypes;
using Refresh.GameServer.Types.Data;

namespace Refresh.GameServer.Types.Challenges.LbpHub;

[XmlRoot("challenge-score")]
[XmlType("challenge-score")]
public class SerializedChallengeScore : SerializedChallengeAttempt, IDataConvertableFrom<SerializedChallengeScore, GameChallengeScore>
{
    [XmlElement("player")] public string PublisherName { get; set; } = SystemUsers.UnknownUserName;
    [XmlElement("rank")] public int Rank { get; set; }

    public static SerializedChallengeScore? FromOld(GameChallengeScore? old, DataContext dataContext)
        => FromOld(old);

    public static SerializedChallengeScore? FromOld(GameChallengeScore? old, int rank = 1, bool fakeScore = false)
    {
        if (old == null)
            return null;

        return new SerializedChallengeScore
        {
            GhostHash = old.GhostHash,
            Score = fakeScore ? 0 : old.Score,
            PublisherName = old.Publisher.Username,
            Rank = rank,
        };
    }

    public static SerializedChallengeScore? FromOld(GameChallengeScoreWithRank? old, bool fakeScore = false)
        => old == null ? null : FromOld(old.score, old.rank, fakeScore);

    public static IEnumerable<SerializedChallengeScore> FromOldList(IEnumerable<GameChallengeScore> oldList, DataContext dataContext)
        => oldList.Select((s, i) => FromOld(s, i + 1)!);

    public static IEnumerable<SerializedChallengeScore> FromOldList(IEnumerable<GameChallengeScoreWithRank> oldList, bool fakeScores = false)
        => oldList.Select((s) => FromOld(s, fakeScores)!);
}