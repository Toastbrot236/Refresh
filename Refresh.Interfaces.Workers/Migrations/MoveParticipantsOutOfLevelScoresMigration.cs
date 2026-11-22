using Microsoft.EntityFrameworkCore;
using Refresh.Common;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Database.Models.Users;
using Refresh.Workers;

namespace Refresh.Interfaces.Workers.Migrations;

public class MoveParticipantsOutOfLevelScoresMigration : MigrationJob<GameScore>
{
    protected override IQueryable<GameScore> SortAndFilter(IQueryable<GameScore> query)
    {
        return query
            .Include(s => s.Publisher)
            .Where(p => p.Participants.Count == 0)
            .OrderBy(p => p.ScoreId);
    }

    protected override int Migrate(WorkContext context, GameScore[] batch)
    {
        foreach (GameScore score in batch)
        {
            // Extra check to be sure
            if (context.Database.GetTotalPlayersFromScoreCount(score) > 0)
                continue;
            
            // Until now we've always ensured that the publisher was also a score participant,
            // so we can avoid looking them up in the database by using score.Publisher, which we include above.
            context.Database.AddScoreParticipant(score, 1, score.Publisher.Username, score.Publisher, false);
            int playerNumber = 2;

            #pragma warning disable CS0618 // obsoletion

            // Don't iterate over publisher's ID, since we've already handled them separately
            foreach (string playerId in score.PlayerIdsRaw.Where(u => u != score.PublisherId.ToString()))
            {
                GameUser? player = context.Database.GetUserByUuid(playerId);
                if (player == null)
                {
                    context.Logger.LogWarning(RefreshContext.Worker, $"Score #{score.ScoreId}: Could not find participant by UUID {playerId} in database!");
                    continue; // No need to increment player number
                }

                context.Database.AddScoreParticipant(score, playerNumber, player.Username, player, false);
            }

            #pragma warning restore CS0618
        }

        context.Database.SaveChanges();
        return batch.Length;
    }
}