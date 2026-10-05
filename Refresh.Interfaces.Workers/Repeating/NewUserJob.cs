using Refresh.Common;
using Refresh.Core.Configuration;
using Refresh.Database;
using Refresh.Database.Models.Users;
using Refresh.Workers;

namespace Refresh.Interfaces.Workers.Repeating;

/// <summary>
/// A job that handles promoting new users to regular users, depending on various account data and configurable requirements.
/// </summary>
// TODO also set users back as "new" if duration in config is updated to result in user being "new" again, maybe
public class NewUserJob : RepeatingJob
{
    protected readonly NewAccountPromotionRequirements _requirements;
    protected override int Interval => 60_000 * 5; // 5 minutes, no need to execute too often
    
    public NewUserJob(NewAccountPromotionRequirements requirements)
    {
        this._requirements = requirements;
    }

    public override void ExecuteJob(WorkContext context)
    {
        DateTimeOffset now = context.TimeProvider.Now;
        DatabaseList<GameUser> newUsers = context.Database.GetAllUsersWithRole(GameUserRole.NewUser);

        foreach (GameUser user in newUsers.Items.ToList())
        {
            // If an account is, e.g., 2 hours and 40 minutes old, and max age for new users is 3 hours, we wouldn't
            // consider max to be reached yet, so floor the difference.
            long accountAge = (long)Math.Floor(now.Subtract(user.JoinDate).TotalHours);
            long requiredPlayTimeMins = this._requirements.ActivePlayTimeHours * 60;

            context.Logger.LogDebug(RefreshContext.Worker, $"{nameof(NewUserJob)} - new user: {user}, current time: {now}, join date: {user.JoinDate}, \n"
                + $"\tAccount age: {accountAge}h/{this._requirements.AccountAgeHours}h, \n"
                + $"\tCached statistics: {user.Statistics == null ? "null, stats will show as -1" : "not null, stats will show properly"}, \n"
                + $"\tTotal play time: {user.Statistics?.TotalPlayTimeMinutes ?? -1}min/{this._requirements.ActivePlayTimeHours * 60}min, \n"
                + $"\tTotal plays: {user.Statistics?.TotalPlayCount ?? -1}/{this._requirements.TotalLevelPlays}, \n"
                + $"\tUnique plays: {user.Statistics?.UniquePlayCount ?? -1}/{this._requirements.UniqueLevelPlays * 60}, \n"
                + $"\tTotal completions: {user.Statistics?.TotalCompletionCount ?? -1}/{this._requirements.TotalLevelCompletions}, \n"
                + $"\tUnique completions: {user.Statistics?.UniqueCompletionCount ?? -1}/{requiredPlayTimeMins}.");

            if (accountAge < this._requirements.AccountAgeHours) continue;
            if (user.Statistics == null) continue; // No need to recalculate here, should be recalculated whenever the stats below change.

            if (user.Statistics.TotalPlayTimeMinutes < requiredPlayTimeMins) continue;
            if (user.Statistics.TotalLevelPlayCount < this._requirements.TotalLevelPlays) continue;
            if (user.Statistics.TotalLevelCompletionCount < this._requirements.TotalLevelCompletions) continue;
            if (user.Statistics.UniqueLevelPlayCount < this._requirements.UniqueLevelPlays) continue;
            if (user.Statistics.UniqueLevelCompletionCount < this._requirements.UniqueLevelCompletions) continue;

            context.Logger.LogInfo(RefreshContext.Worker, $"Promoting {user} to regular user since their account meets all requirements now.");
            context.Database.SetUserRole(user, GameUserRole.User);
        }
    }
}