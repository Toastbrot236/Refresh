using Microsoft.EntityFrameworkCore;
using Refresh.Common;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Database.Models.Users;
using Refresh.Workers;

namespace Refresh.Interfaces.Workers.Migrations;

public class MovePlayersOutOfLevelScoresMigration : MigrationJob<GameScore>
{
    protected override IQueryable<GameScore> SortAndFilter(IQueryable<GameScore> query)
    {
        return query
            .Include(s => s.Publisher)
            .OrderBy(p => p.ScoreId);
    }

    protected override int Migrate(WorkContext context, GameScore[] batch)
    {
        foreach (GameScore score in batch)
        {
            // Extra check to be sure, this check only uses the new table to count
            if (context.Database.GetTotalPlayersFromScoreCount(score) > 0)
                continue;

            int playerNumber = 1;
            string publisherIdStr = score.PublisherId.ToString();

            #pragma warning disable CS0618 // obsoletion

            foreach (string playerId in score.PlayerIdsRaw)
            {
                // Do iterate over publisher too, because they're not guaranteed to be the first player, but also avoid looking their
                // GameUser up in DB because we already have it.
                GameUser? player = playerId == publisherIdStr
                    ? score.Publisher
                    : context.Database.GetUserByUuid(playerId);

                if (player == null)
                {
                    context.Logger.LogWarning(RefreshContext.Worker, $"Score #{score.ScoreId}: Could not find player by UUID {playerId} in database! Their participance on this score will not be migrated.");
                }
                else
                {
                    context.Database.AddScorePlayer(score, playerNumber, player, false);
                }

                playerNumber++;
            }

            #pragma warning restore CS0618
        }

        context.Database.SaveChanges();
        return batch.Length;
    }
}