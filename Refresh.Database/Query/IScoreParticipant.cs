using Refresh.Database.Models.Users;

namespace Refresh.Database.Query;

public interface IScorePlayer
{
    string Username { get; }
    GameUser User { get; }
}