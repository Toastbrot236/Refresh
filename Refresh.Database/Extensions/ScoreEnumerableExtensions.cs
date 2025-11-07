using Refresh.Database.Models.Levels.Scores;

namespace Refresh.Database.Extensions;

public static class ScoreEnumerableExtensions
{
    public static IEnumerable<ScoreWithRank> RankScores(this IEnumerable<GameScore> scores)
    {
        // Scores with the same value should have the same rank for more consistent behaviour as in-game.
        // Also makes multiplayer score ranking fairer, as every player in a lobby uploads a copy of their
        // score themselves.
        IEnumerable<GameScore> sorted = scores.OrderByDescending(s => s.Score);
        int currentRank = 1;
        int previousScore = sorted.FirstOrDefault()?.Score ?? 0;
        List<ScoreWithRank> rankedScores = [];
        foreach (GameScore score in sorted.OrderByDescending(s => s.Score))
        {
            if (score.Score < previousScore)
            {
                previousScore = score.Score;
                currentRank++;
            }

            rankedScores.Add(new(score, currentRank));
        }

        return rankedScores;
    }
}