using Refresh.Database.Models.Assets.CWLib;
using Refresh.Database.Models.Authentication;

namespace Refresh.Database;

public partial class GameDatabaseContext // CWLib
{
    public FarcHashesData? GetNewestFarcHashes(TokenGame? specificGame)
    {
        return this.FarcHashesData
            .Where(f => f.Version > 0)
            .OrderByDescending(f => f.GeneratedAt)
            .FirstOrDefault();
    }
}