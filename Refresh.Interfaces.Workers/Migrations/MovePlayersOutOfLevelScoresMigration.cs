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
            .Where(p => p.Players.Count == 0)
            .OrderBy(p => p.ScoreId);
    }

    protected override void Migrate(WorkContext context, GameScore[] batch)
    {
        context.Logger.LogInfo(RefreshContext.Worker, $"Migrating {batch.Length} scores");

        foreach (GameScore score in batch)
        {
            context.Logger.LogInfo(RefreshContext.Worker, $"Migrating score {score.ScoreId} for level {score.LevelId}/type {score.ScoreType} and publisher {score.Publisher}, legacy player count {score.PlayerIdsRaw.Count}");

            // Extra check to be sure
            if (context.Database.GetTotalPlayersFromScoreCount(score) > 0)
                continue;
            
            // Until now we've always ensured that the publisher was also a score player/participant,
            // so we can avoid looking them up in the database by using score.Publisher, which we include above.
            int playerNumber = 1;
            context.Database.AddScorePlayer(score, playerNumber, score.Publisher.Username, score.Publisher, false);

            #pragma warning disable CS0618 // obsoletion

            // Don't iterate over publisher's ID, since we've already handled them separately
            foreach (string playerId in score.PlayerIdsRaw.Where(u => u != score.PublisherId.ToString()))
            {
                playerNumber++;

                GameUser? player = context.Database.GetUserByUuid(playerId);
                if (player == null)
                {
                    context.Logger.LogWarning(RefreshContext.Worker, $"Score #{score.ScoreId}: Could not find player by UUID {playerId} in database! Their participance on this score will not be migrated.");
                }
                else
                {
                    context.Database.AddScorePlayer(score, playerNumber, player.Username, player, false);
                }
            }

            #pragma warning restore CS0618
        }

        context.Database.SaveChanges();
    }
}