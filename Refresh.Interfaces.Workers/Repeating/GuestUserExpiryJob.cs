using Refresh.Core;
using Refresh.Core.Configuration;
using Refresh.Database.Models.Users;
using Refresh.Workers;

namespace Refresh.Interfaces.Workers.Repeating;

/// <summary>
/// A worker that deletes (expires) guest accounts who have been taking too long
/// </summary>
public class GuestUserExpiryJob : RepeatingJob
{
    protected override int Interval => 60_000; // 1 minute
    private readonly int _hoursToLive;

    public GuestUserExpiryJob(GameServerConfig gameConfig)
    {
        this._hoursToLive = gameConfig.GuestAccountTimeToLiveInHours;
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
            context.Logger.LogInfo(RefreshContext.Worker, $"Deleted {user.Username}'s guest account since they are seemingly no longer online");
        }
    }
}