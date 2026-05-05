using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Users;
using Refresh.Database.Models.Relations;

namespace Refresh.Database;

public partial class GameDatabaseContext // Integrations
{
    private IQueryable<CachedOAuthCodeRelation> CachedOAuthCodeRelationsIncluded => this.CachedOAuthCodeRelations
        .Include(c => c.User);

    public bool IsOAuthCodeCached(string code, OAuthAuthority fromAuthority)
        => CachedOAuthCodeRelations.Any(c => c.Code == code && c.FromAuthority == fromAuthority);

    public CachedOAuthCodeRelation? GetOAuthCodeDataFromCache(string code, OAuthAuthority fromAuthority)
        => CachedOAuthCodeRelationsIncluded.FirstOrDefault(c => c.Code == code && c.FromAuthority == fromAuthority);
    
    public GameUser? GetUserFromCachedOAuthCode(string code, OAuthAuthority fromAuthority)
        => this.GetOAuthCodeDataFromCache(code, fromAuthority)?.User;

    public DatabaseList<CachedOAuthCodeRelation> GetAllCachedOAuthCodes(OAuthAuthority? fromAuthority, GameUser? forUser, int skip, int count)
    {
        IQueryable<CachedOAuthCodeRelation> allRelations = this.CachedOAuthCodeRelationsIncluded;

        if (fromAuthority != null) allRelations = allRelations.Where(c => c.FromAuthority == fromAuthority);
        if (forUser != null) allRelations = allRelations.Where(c => c.UserId == forUser.UserId);

        return new(allRelations, skip, count);
    }

    /// <remarks>
    /// Does not ensure this code isn't already cached, so validate this in the calling method.
    /// </remarks>
    public CachedOAuthCodeRelation AddOAuthCodeToCache(string code, OAuthAuthority fromAuthority, GameUser forUser, int cacheExpirySeconds = DefaultTokenExpirySeconds)
    {
        DateTimeOffset now = this._time.Now;

        CachedOAuthCodeRelation newRelation = new()
        {
            Code = code,
            FromAuthority = fromAuthority,
            User = forUser,
            ReceivedAt = now,
            ExpiresAt = now.AddSeconds(cacheExpirySeconds),
        };
        this.CachedOAuthCodeRelations.Add(newRelation);
        this.SaveChanges();
        return newRelation;
    }

    public void RemoveOAuthCodeFromCache(CachedOAuthCodeRelation relation, bool save = true)
    {
        this.CachedOAuthCodeRelations.Remove(relation);
        if (save) this.SaveChanges();
    }
}