using Refresh.Database.Models.Users;

namespace Refresh.Database.Query;

public interface IScoreParticipant
{
    string Username { get; }
    GameUser User { get; }
}