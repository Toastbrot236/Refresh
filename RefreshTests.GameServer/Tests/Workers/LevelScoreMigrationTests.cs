using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Levels.Scores;
using Refresh.Database.Models.Users;
using Refresh.Database.Models.Workers;
using Refresh.Interfaces.Workers.Migrations;
using Refresh.Workers.State;

namespace RefreshTests.GameServer.Tests.Workers;

public class LevelScoreMigrationTests : GameServerTest
{
    private void RunJob(TestContext context, int scoreCount)
    {
        MovePlayersOutOfLevelScoresMigration job = new();
        context.Database.UpdateOrCreateJobState(typeof(MovePlayersOutOfLevelScoresMigration).Name, new MigrationJobState()
        {
            Total = scoreCount
        }, WorkerClass.Refresh);
        context.Database.Refresh();

        object? stateObject = context.Database.GetJobState(typeof(MovePlayersOutOfLevelScoresMigration).Name, typeof(MigrationJobState), WorkerClass.Refresh);
        Assert.That(stateObject, Is.Not.Null);
        job.JobState = stateObject!;
        
        // Migrate - First cycle
        job.ExecuteJob(context.GetWorkContext());
        context.Database.Refresh();
    }

    [Test]
    public void MigratesAllScorePlayersButNotMultipleTimes()
    {
        // We easily use this test to also ensure that non-highscores get migrated too,
        // scores don't get migrated more than once, and that all players are correctly migrated.
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        GameUser friend = context.CreateUser();
        GameLevel level1 = context.CreateLevel(user);

        // Add new, unmigrated scores
        context.Database.GameScores.Add(new GameScore()
            {
                Game = TokenGame.LittleBigPlanet1,
                Platform = TokenPlatform.RPCS3,
                LevelId = level1.LevelId,
                ScoreSubmitted = context.Time.Now,
                Score = 420,
                ScoreType = 3,
                PublisherId = user.UserId,
                Rank = 1,

                #pragma warning disable CS0618 // obsoletion
                PlayerIdsRaw = [user.UserId.ToString(), friend.UserId.ToString()],
                #pragma warning restore CS0618
            });
        context.Database.SaveChanges();

        // First migration cycle

        this.RunJob(context, 2);

        IEnumerable<ScoreWithRank> userScores = context.Database.GetTopScoresByUser(user, 50, 0, 1, showDuplicates: true).Items;
        Assert.That(userScores.Count(), Is.EqualTo(5));

        foreach (ScoreWithRank userScore in userScores)
        {
            Assert.That(userScore.score.PublisherId.ToString(), Is.EqualTo(user.UserId.ToString()));
            Assert.That(userScore.score.Players.Count, Is.EqualTo(2)); // Migrated rows are included now
            Assert.That(context.Database.GetTotalPlayersFromScoreCount(userScore.score), Is.EqualTo(2)); // Migrated rows are fetched now

            // Ensure the included user data is correct (only need to check included data)
            GameScorePlayer? userPlayer = userScore.score.Players.FirstOrDefault(p => p.UserId.ToString() == user.UserId.ToString());
            GameScorePlayer? friendPlayer = userScore.score.Players.FirstOrDefault(p => p.UserId.ToString() == friend.UserId.ToString());

            Assert.That(userPlayer, Is.Not.Null);
            Assert.That(friendPlayer, Is.Not.Null);

            Assert.That(userPlayer.Username, Is.EqualTo(user.Username));
            Assert.That(friendPlayer.Username, Is.EqualTo(user.Username));

            Assert.That(userPlayer.User!.UserId.ToString(), Is.EqualTo(user.UserId.ToString()));
            Assert.That(friendPlayer.User!.UserId.ToString(), Is.EqualTo(friend.UserId.ToString()));
        }

        // Add a few new scores, and then re-migrate. The RunJob() method will effectively reset job state for us,
        // so we can be sure that the job will try to re-migrate already migrated scores.
        // Test whether the job execution itself has skipped migration of such scores by counting the player rows.
        GameLevel level2 = context.CreateLevel(friend);

        context.Database.GameScores.Add(new GameScore()
            {
                Game = TokenGame.LittleBigPlanet2,
                Platform = TokenPlatform.PS3,
                LevelId = level2.LevelId,
                ScoreSubmitted = context.Time.Now,
                Score = 420,
                ScoreType = 3,
                PublisherId = user.UserId,
                Rank = 1,

                #pragma warning disable CS0618 // obsoletion
                PlayerIdsRaw = [user.UserId.ToString(), friend.UserId.ToString()],
                #pragma warning restore CS0618
            });
        context.Database.SaveChanges();

        // Second migration cycle
        
        this.RunJob(context, 2);
        
        userScores = context.Database.GetTopScoresByUser(user, 50, 0, 1, showDuplicates: true).Items;
        Assert.That(userScores.Count(), Is.EqualTo(10));

        foreach (ScoreWithRank userScore in userScores)
        {
            // Player counts should still be 2 for all scores
            Assert.That(userScore.score.PublisherId.ToString(), Is.EqualTo(user.UserId.ToString()));
            Assert.That(userScore.score.Players.Count, Is.EqualTo(2)); // All scores still have 2 players, even the first 5 scores
            Assert.That(context.Database.GetTotalPlayersFromScoreCount(userScore.score), Is.EqualTo(2));

            // Ensure the included user data is (still) correct on all scores
            GameScorePlayer? userPlayer = userScore.score.Players.FirstOrDefault(p => p.UserId.ToString() == user.UserId.ToString());
            GameScorePlayer? friendPlayer = userScore.score.Players.FirstOrDefault(p => p.UserId.ToString() == friend.UserId.ToString());

            Assert.That(userPlayer, Is.Not.Null);
            Assert.That(friendPlayer, Is.Not.Null);

            Assert.That(userPlayer.Username, Is.EqualTo(user.Username));
            Assert.That(friendPlayer.Username, Is.EqualTo(user.Username));

            Assert.That(userPlayer.User!.UserId.ToString(), Is.EqualTo(user.UserId.ToString()));
            Assert.That(friendPlayer.User!.UserId.ToString(), Is.EqualTo(friend.UserId.ToString()));
        }
    }
}