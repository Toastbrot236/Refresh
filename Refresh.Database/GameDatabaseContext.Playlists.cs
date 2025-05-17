using Refresh.Database.Models;
using Refresh.Database.Query;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Users;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Playlists;
using Refresh.Database.Models.Relations;

namespace Refresh.Database;

public partial class GameDatabaseContext // Playlists
{
    /// <summary>
    /// Default icon used by playlists created in LBP3, through ApiV3 or similar
    /// </summary>
    private const string DefaultPlaylistIcon = "g18451"; // LBP1 star sticker

    private void CreatePlaylistInternal(GamePlaylist createInfo)
    {
        DateTimeOffset now = this._time.Now;

        this.AddSequentialObject(createInfo, () =>
        {
            createInfo.CreationDate = now;
            createInfo.LastUpdateDate = now;
        });
    }

    public GamePlaylist CreatePlaylist(GameUser user, ISerializedCreatePlaylistInfo createInfo, bool rootPlaylist = false)
    {
        GameLocation location = createInfo.Location ?? GameLocation.Random;
        
        GamePlaylist playlist = new() 
        {
            Publisher = user,
            Name = createInfo.Name ?? "",
            Description = createInfo.Description ?? "",
            IconHash = createInfo.Icon ?? DefaultPlaylistIcon,
            LocationX = location.X,
            LocationY = location.Y,
            IsRoot = rootPlaylist,
        };

        this.CreatePlaylistInternal(playlist);

        return playlist;
    }

    public void CreateRootPlaylist(GameUser user)
    {
        GameLocation randomLocation = GameLocation.Random;

        GamePlaylist rootPlaylist = new()
        {
            Publisher = user,
            Name = "My Playlists",
            Description = $"{user.Username}'s root playlist",
            IconHash = DefaultPlaylistIcon,
            LocationX = randomLocation.X,
            LocationY = randomLocation.Y,
            IsRoot = true,
        };

        this.CreatePlaylistInternal(rootPlaylist);
        this.SetUserRootPlaylist(user, rootPlaylist);
    }

    public GamePlaylist? GetPlaylistById(int playlistId) 
        => this.GamePlaylists.FirstOrDefault(p => p.PlaylistId == playlistId);

    public void UpdatePlaylist(GamePlaylist playlist, ISerializedCreatePlaylistInfo updateInfo)
    {
        GameLocation location = updateInfo.Location ?? new GameLocation(playlist.LocationX, playlist.LocationY);
        
        this.Write(() =>
        {
            playlist.Name = updateInfo.Name ?? playlist.Name;
            playlist.Description = updateInfo.Description ?? playlist.Description;
            playlist.IconHash = updateInfo.Icon ?? playlist.IconHash;
            playlist.LocationX = location.X;
            playlist.LocationY = location.Y;
            playlist.LastUpdateDate = this._time.Now;
        });
    }

    public void DeletePlaylist(GamePlaylist playlist)
    {
        this.Write(() =>
        {
            // Remove all relations relating to this playlist
            this.LevelPlaylistRelations.RemoveRange(l => l.Playlist == playlist);
            this.SubPlaylistRelations.RemoveRange(l => l.Playlist == playlist || l.SubPlaylist == playlist);
            this.FavouritePlaylistRelations.RemoveRange(l => l.Playlist == playlist);
            
            // Remove the playlist object
            this.GamePlaylists.Remove(playlist);
        });
    }

    public void AddPlaylistToPlaylist(GamePlaylist child, GamePlaylist parent)
    {
        this.Write(() =>
        {
            // Make sure to not create a duplicate object
            if (this.SubPlaylistRelations.Any(p => p.SubPlaylist == child && p.Playlist == parent))
                return;
            
            DateTimeOffset now = this._time.Now;
            
            // Add the relation
            this.SubPlaylistRelations.Add(new SubPlaylistRelation
            {
                Playlist = parent,
                SubPlaylist = child,
                Timestamp = now,
            });

            // The parent playlist has been updated
            parent.LastUpdateDate = now;
        });
    }
    
    public void RemovePlaylistFromPlaylist(GamePlaylist child, GamePlaylist parent)
    {
        this.Write(() =>
        {
            SubPlaylistRelation? relation =
                this.SubPlaylistRelations.FirstOrDefault(r => r.SubPlaylist == child && r.Playlist == parent);

            if (relation == null)
                return;
            
            this.SubPlaylistRelations.Remove(relation);
        });
    }

    /// <remarks>
    /// This method assumes it is called inside a write operation
    /// </remarks>
    private void AddLevelToPlaylistInternal(GameLevel level, GamePlaylist parent, DateTimeOffset timestamp, int index)
    {
        this.LevelPlaylistRelations.Add(new LevelPlaylistRelation
        {
            Level = level,
            Playlist = parent,
            Index = index,
            Timestamp = timestamp,
        });
    }
    
    public void AddLevelToPlaylist(GameLevel level, GamePlaylist parent)
    {
        // Make sure to not create a duplicate object
        if (this.LevelPlaylistRelations.Any(p => p.Level == level && p.Playlist == parent))
            return;
        
        DateTimeOffset now = this._time.Now;

        this.Write(() =>
        {
            // Increment all other relations' index by 1
            this.IncrementPlaylistLevelIndicesInternal(parent, 1);

            // Create the new relation with an index which will put its level infront of all others
            this.AddLevelToPlaylistInternal(level, parent, now, 0);

            // The parent playlist has been updated
            parent.LastUpdateDate = now;
        });
    }

    public void AddLevelsToPlaylist(IEnumerable<int> levelIds, GamePlaylist parent)
    {
        // Ensure these levels actually do exist
        IEnumerable<GameLevel> levelsToAdd = this.GetLevelsByIdsInternal(levelIds);
        int levelsToAddCount = levelsToAdd.Count();
        DateTimeOffset now = this._time.Now;
        
        this.Write(() => 
        {
            // Increment every already existing playlist level relations' index by levelsToAddCount
            this.IncrementPlaylistLevelIndicesInternal(parent, levelsToAddCount);

            // Now add the relations with indices which will put their levels infront of all others
            int index = 0;
            foreach (GameLevel level in levelsToAdd)
            {
                // Make sure to not create a duplicate object
                if (this.LevelPlaylistRelations.Any(p => p.Level == level && p.Playlist == parent))
                    continue;
                
                this.AddLevelToPlaylistInternal(level, parent, now, index);
                index++;
            }

            // The parent playlist has been updated
            parent.LastUpdateDate = now;
        });

        int levelIdsCount = levelIds.Count();
        if (levelsToAddCount < levelIdsCount)
        {
            this.AddErrorNotification
            (
                "Failed to add levels to playlist",
                $"Failed to add {levelIdsCount - levelsToAddCount} out of {levelIdsCount} levels to playlist {parent.Name} " +
                $"because they couldn't be found on the server.",
                parent.Publisher
            );
        }
    }

    /// <remarks>
    /// This method assumes it is called inside a write operation
    /// </remarks>
    private void IncrementPlaylistLevelIndicesInternal(GamePlaylist parent, int summand)
    {
        IEnumerable<LevelPlaylistRelation> relations = this.GetLevelRelationsForPlaylist(parent);

        foreach (LevelPlaylistRelation relation in relations)
        {
            relation.Index += summand;
        }
    }

    public void ReorderLevelsInPlaylist(IEnumerable<int> levelIds, GamePlaylist parent)
    {
        IEnumerable<LevelPlaylistRelation> relations = this.GetLevelRelationsForPlaylist(parent);
        IEnumerable<LevelPlaylistRelation> includedRelations = relations.Where(r => levelIds.Contains(r.Level.LevelId));
        IEnumerable<LevelPlaylistRelation> excludedRelations = relations.Where(r => !levelIds.Contains(r.Level.LevelId));
        int failedUpdates = 0;

        this.Write(() => 
        {
            // update playlist levels referenced in the given list
            int newIndex = 0;
            foreach (int levelId in levelIds)
            {
                LevelPlaylistRelation? includedRelation = includedRelations.FirstOrDefault(r => r.Level.LevelId == levelId);

                // only update if the playlist actually contains the level
                if (includedRelation != null)
                {
                    includedRelation.Index = newIndex;
                    newIndex++;
                }
                else
                {
                    failedUpdates++;
                }
            }

            // update levels not included in the list to retain their previously set order, but to be behind the newly ordered levels
            foreach (LevelPlaylistRelation excludedRelation in excludedRelations)
            {
                excludedRelation.Index = newIndex;
                newIndex++;
            }
        });

        if (failedUpdates > 0)
        {
            this.AddErrorNotification
            (
                "Failed to reorder levels in playlist",
                $"Failed to reorder {failedUpdates} out of {includedRelations.Count()} levels in playlist {parent.Name} " +
                $"because they were not in the playlist. Please add them first.",
                parent.Publisher
            );
        }
    }

    public void RemoveLevelFromPlaylist(GameLevel level, GamePlaylist parent)
    {
        LevelPlaylistRelation? relation =
            this.LevelPlaylistRelations.FirstOrDefault(r => r.Level == level && r.Playlist == parent);

        if (relation == null)
            return;

        this.Write(() =>
        {
            this.LevelPlaylistRelations.Remove(relation);
        });
    }

    public IEnumerable<GamePlaylist> GetPlaylistsContainingPlaylist(GamePlaylist playlist)
        // TODO: with postgres this can be IQueryable
        => this.SubPlaylistRelations.Where(p => p.SubPlaylist == playlist).OrderByDescending(r => r.Timestamp)
            .AsEnumerable()
            .Select(r => this.GamePlaylists.First(p => p.PlaylistId == r.Playlist.PlaylistId))
            .Where(p => !p.IsRoot);

    public DatabaseList<GamePlaylist> GetPlaylistsContainingPlaylist(GamePlaylist playlist, int skip, int count)
        => new(this.GetPlaylistsContainingPlaylist(playlist), skip, count);
    
    public DatabaseList<GamePlaylist> GetPlaylistsByAuthorContainingPlaylist(GameUser user, GamePlaylist playlist, int skip, int count)
        // TODO: with postgres this can be IQueryable
        => new(this.SubPlaylistRelations
            .Where(p => p.SubPlaylist == playlist)
            .OrderByDescending(r => r.Timestamp)
            .AsEnumerable()
            .Select(r => this.GamePlaylists.First(p => p.PlaylistId == r.Playlist.PlaylistId))
            .Where(p => p.Publisher.UserId == user.UserId)
            .Where(p => !p.IsRoot), skip, count);
    
    private IEnumerable<LevelPlaylistRelation> GetLevelRelationsForPlaylist(GamePlaylist playlist)
        => this.LevelPlaylistRelations
            .Where(l => l.Playlist == playlist)
            .OrderBy(r => r.Index);
        
    public DatabaseList<GameLevel> GetLevelsInPlaylist(GamePlaylist playlist, TokenGame game, int skip, int count)
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance. 
        => new(GetLevelRelationsForPlaylist(playlist)
            .AsEnumerable()
            .Select(l => l.Level)
            .FilterByGameVersion(game), skip, count);

    public DatabaseList<GameLevel> GetLevelsInPlaylist(GamePlaylist playlist, int skip, int count)
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance. 
        => new(GetLevelRelationsForPlaylist(playlist)
            .AsEnumerable()
            .Select(l => l.Level), skip, count);

    public int GetTotalLevelsInPlaylistCount(GamePlaylist playlist, TokenGame game) => 
        this.LevelPlaylistRelations.Where(l => l.Playlist == playlist)
            .AsEnumerable()
            .Select(l => l.Level)
            .FilterByGameVersion(game)
            .Count();

    public int GetTotalLevelsInPlaylistCount(GamePlaylist playlist) => 
        this.LevelPlaylistRelations.Count(l => l.Playlist == playlist);

    public DatabaseList<GamePlaylist> GetPlaylistsInPlaylist(GamePlaylist playlist, int skip, int count)
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance. 
        => new(this.SubPlaylistRelations
            .Where(p => p.Playlist == playlist)
            .OrderByDescending(r => r.Timestamp)
            .AsEnumerable()
            .Select(l => l.SubPlaylist), skip, count);
    
    public DatabaseList<GamePlaylist> GetPlaylistsByAuthor(GameUser author, int skip, int count)
        => new(this.GamePlaylists
            .Where(p => p.Publisher == author)
            .Where(p => !p.IsRoot)
            .OrderByDescending(p => p.LastUpdateDate), skip, count);

    public DatabaseList<GamePlaylist> GetPlaylistsByAuthorContainingLevel(GameUser author, GameLevel level, int skip, int count)
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance. 
        => new(this.LevelPlaylistRelations
            .Where(p => p.Level == level)
            .OrderByDescending(r => r.Timestamp)
            .AsEnumerable()
            .Select(r => this.GamePlaylists.First(p => p.PlaylistId == r.Playlist.PlaylistId))
            .Where(p => p.Publisher.UserId == author.UserId), skip, count);
    
    public DatabaseList<GamePlaylist> GetPlaylistsContainingLevel(GameLevel level, int skip, int count)
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance. 
        => new(this.LevelPlaylistRelations
            .Where(p => p.Level == level)
            .OrderByDescending(r => r.Timestamp)
            .AsEnumerable()
            .Select(r => this.GamePlaylists.First(p => p.PlaylistId == r.Playlist.PlaylistId)), skip, count);

    public DatabaseList<GamePlaylist> GetNewestPlaylists(int skip, int count)
        => new(this.GamePlaylists
            .Where(p => !p.IsRoot)
            .OrderByDescending(p => p.CreationDate), skip, count);

    public DatabaseList<GamePlaylist> GetMostHeartedPlaylists(int skip, int count) 
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance.
        // TODO: reduce code duplication for getting most of x
        => new(this.FavouritePlaylistRelations
            .GroupBy(r => r.Playlist)
            .Select(g => new { Playlist = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .AsEnumerable()
            .Select(x => x.Playlist)
            .Where(p => p != null), skip, count);

    public DatabaseList<GamePlaylist> GetPlaylistsFavouritedByUser(GameUser user, int skip, int count) 
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance.
        => new(this.FavouritePlaylistRelations
            .Where(r => r.User == user)
            .OrderByDescending(r => r.Timestamp)
            .AsEnumerable()
            .Select(r => r.Playlist), skip, count);

    public int GetFavouriteCountForPlaylist(GamePlaylist playlist)
        => this.FavouritePlaylistRelations.Count(r => r.Playlist == playlist);

    public bool IsPlaylistFavouritedByUser(GamePlaylist playlist, GameUser user)
        => this.FavouritePlaylistRelations.FirstOrDefault(r => r.Playlist == playlist && r.User == user) != null;

    public bool FavouritePlaylist(GamePlaylist playlist, GameUser user)
    {
        if (this.IsPlaylistFavouritedByUser(playlist, user)) return false;

        FavouritePlaylistRelation relation = new()
        {
            Playlist = playlist,
            User = user,
            Timestamp = this._time.Now,
        };
        this.Write(() => this.FavouritePlaylistRelations.Add(relation));

        return true;
    }

    public bool UnfavouritePlaylist(GamePlaylist playlist, GameUser user)
    {
        FavouritePlaylistRelation? relation = this.FavouritePlaylistRelations
            .FirstOrDefault(r => r.Playlist == playlist && r.User == user);

        if (relation == null) return false;

        this.Write(() => this.FavouritePlaylistRelations.Remove(relation));

        return true;
    }
}