using Refresh.Core;
using Refresh.Core.Configuration;
using Refresh.Database.Models.Users;
using Refresh.Workers;

namespace Refresh.Interfaces.Workers.Repeating;

/// <summary>
/// A worker which deletes (expires) guest accounts who have been taking too long to get online again
/// </summary>
public class GuestUserExpiryJob : RepeatingJob
{
    protected override int Interval => 300_000; // 5 minutes
    private readonly int _hoursToLive;

    public GuestUserExpiryJob(GameServerConfig gameConfig)
    {
        this._hoursToLive = Math.Clamp(gameConfig.GuestAccountHoursToLiveAfterLastContact, 1, 72); // 1 hour - 3 days
    }

    public override void ExecuteJob(WorkContext context)
    {
        DateTimeOffset now = DateTimeOffset.Now;

        GameUser[] guestsToExpire = context.Database.GetAllUsersWithRole(GameUserRole.Guest).Items
            .Where(u => u.LastGameContactDate.AddHours(_hoursToLive) < now)
            .ToArray();
        
        foreach (GameUser user in guestsToExpire)
        {
            context.Database.FullyDeleteUser(user);
            context.Logger.LogInfo(RefreshContext.Worker, $"Deleted {user.Username}'s guest account since it has expired {DateTimeOffset.Now - user.LastGameContactDate} ago");
        }
    }
}