using Refresh.GameServer.Authentication;
using Refresh.GameServer.Extensions;
using Refresh.GameServer.Types;
using Refresh.GameServer.Types.Levels;
using Refresh.GameServer.Types.Playlists;
using Refresh.GameServer.Types.Relations;
using Refresh.GameServer.Types.UserData;

namespace Refresh.GameServer.Database;

public partial class GameDatabaseContext // Playlists
{
    /// <summary>
    /// Default icon used by playlists created in LBP3, through ApiV3 or similar
    /// </summary>
    private const string defaultPlaylistIcon = "g18451"; // LBP1 star sticker

    private void CreatePlaylistInternal(GamePlaylist createInfo)
    {
        DateTimeOffset now = this._time.Now;

        this.AddSequentialObject(createInfo, () =>
        {
            createInfo.CreationDate = now;
            createInfo.LastUpdateDate = now;
        });
    }

    public GamePlaylist CreatePlaylist(GameUser user, SerializedLbp1Playlist createInfo, bool rootPlaylist = false)
    {
        GamePlaylist playlist = new() 
        {
            Publisher = user, 
            Name = createInfo.Name,
            Description = createInfo.Description, 
            IconHash = createInfo.Icon, 
            LocationX = createInfo.Location.X, 
            LocationY = createInfo.Location.Y,
            IsRoot = rootPlaylist,
        };

        this.CreatePlaylistInternal(playlist);

        return playlist;
    }

    public GamePlaylist CreatePlaylist(GameUser user, SerializedLbp3Playlist createInfo)
    {
        GameLocation randomLocation = GameLocation.Random;

        GamePlaylist playlist = new()
        {
            Publisher = user, 
            Name = createInfo.Name ?? "",
            Description = createInfo.Description ?? "", 
            IconHash = defaultPlaylistIcon,
            LocationX = randomLocation.X, 
            LocationY = randomLocation.Y,
            IsRoot = false,
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
            IconHash = defaultPlaylistIcon,
            LocationX = randomLocation.X,
            LocationY = randomLocation.Y,
            IsRoot = true
        };

        this.CreatePlaylistInternal(rootPlaylist);
        this.SetUserRootPlaylist(user, rootPlaylist);
    }

    public GamePlaylist? GetPlaylistById(int playlistId) 
        => this.GamePlaylists.FirstOrDefault(p => p.PlaylistId == playlistId);

    public void UpdatePlaylist(GamePlaylist playlist, SerializedLbp1Playlist updateInfo)
    {
        this.Write(() =>
        {
            playlist.Name = updateInfo.Name;
            playlist.Description = updateInfo.Description;
            playlist.IconHash = updateInfo.Icon;
            playlist.LocationX = updateInfo.Location.X;
            playlist.LocationY = updateInfo.Location.Y;
            playlist.LastUpdateDate = this._time.Now;
        });
    }

    public void UpdatePlaylist(GamePlaylist playlist, SerializedLbp3Playlist updateInfo)
    {
        this.Write(() =>
        {
            if (updateInfo.Name != null) playlist.Name = updateInfo.Name;
            if (updateInfo.Description != null) playlist.Description = updateInfo.Description;
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
        // Make sure to not create a duplicate object
        if (this.SubPlaylistRelations.Any(p => p.SubPlaylist == child && p.Playlist == parent))
            return;
        
        this.Write(() =>
        {
            // Add the relation
            this.SubPlaylistRelations.Add(new SubPlaylistRelation
            {
                Playlist = parent,
                SubPlaylist = child,
                Timestamp = this._time.Now,
            });

            // The parent playlist has been updated
            parent.LastUpdateDate = this._time.Now;
        });
    }
    
    public void RemovePlaylistFromPlaylist(GamePlaylist child, GamePlaylist parent)
    {
        SubPlaylistRelation? relation =
            this.SubPlaylistRelations.FirstOrDefault(r => r.SubPlaylist == child && r.Playlist == parent);

        if (relation == null)
            return;

        this.Write(() =>
        {
            this.SubPlaylistRelations.Remove(relation);

            // The parent playlist has been updated
            parent.LastUpdateDate = this._time.Now;
        });
    }
    
    public void AddLevelToPlaylist(GameLevel level, GamePlaylist parent)
    {
        // Make sure to not create a duplicate object
        if (this.LevelPlaylistRelations.Any(p => p.Level == level && p.Playlist == parent))
            return;

        IEnumerable<LevelPlaylistRelation> previousRelations = this.GetLevelPlaylistRelationsForPlaylist(parent);

        // index of new relation = index of last relation + 1 = relation count (without new relation)
        int index = this.GetTotalLevelsInPlaylistCount(parent);

        this.Write(() =>
        {
            // Add the relation
            this.AddLevelToPlaylistInternal(level, parent, this._time.Now, 0);

            // Now overwrite the indices of all already previously existing relations
            int newIndex = 1;
            foreach (LevelPlaylistRelation relation in previousRelations)
            {
                this.OverwriteIndexAndIncrement(relation.Index, newIndex);
            }

            // The parent playlist has been updated
            parent.LastUpdateDate = this._time.Now;
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
    
    public void RemoveLevelFromPlaylist(GameLevel level, GamePlaylist parent)
    {
        LevelPlaylistRelation? relation =
            this.LevelPlaylistRelations.FirstOrDefault(r => r.Level == level && r.Playlist == parent);

        if (relation == null)
            return;

        this.Write(() =>
        {
            this.LevelPlaylistRelations.Remove(relation);

            // The parent playlist has been updated
            parent.LastUpdateDate = this._time.Now;

            // There is no need to update the playlist levels' indices due to a "gap" not being able to 
            // change the order of the two level relations around a missing relation in any way, for example.
        });
    }

    public void AddLevelsToPlaylist(GamePlaylist parent, List<int> levelIds, GameUser user)
    {
        IEnumerable<LevelPlaylistRelation> previousRelations = this.GetLevelPlaylistRelationsForPlaylist(parent);
        DateTimeOffset now = this._time.Now;

        int levelIdsCount = levelIds.Count;
        int newIndex = 0;
        int alreadyAddedLevels = 0;
        int nonExistentLevels = 0;
        
        this.Write(() => 
        {
            // Iterate through the level IDs unlike with previousRelations below, 
            // to preserve the reordered levels' order for the new relations' indices
            foreach (int levelId in levelIds)
            {
                LevelPlaylistRelation? relation = previousRelations.FirstOrDefault(r => r.Level.LevelId == levelId);
                if (relation == null)
                {
                    GameLevel? levelToAdd = this.GetLevelById(levelId);
                    // If there is no relation for this level ID, and no level actually exists under this ID, skip it
                    if (levelToAdd == null)
                    {
                        nonExistentLevels++;
                    }
                    // If there is a level under this ID and it's not in the playlist yet, add it and set its index appropriately
                    else
                    {
                        this.AddLevelToPlaylistInternal(levelToAdd, parent, now, newIndex);
                    }
                }
                // If there already is a relation linking a level under this level ID to the playlist, skip it
                else
                {
                    alreadyAddedLevels++;
                }
            }

            // Now overwrite the indices of all already previously existing relations, which is possible thanks to
            // ordering the relations returned by GetLevelPlaylistRelationsForPlaylist() by index
            foreach (LevelPlaylistRelation relation in previousRelations)
            {
                this.OverwriteIndexAndIncrement(relation.Index, newIndex);
            }

            // The parent playlist has been updated
            parent.LastUpdateDate = now;
        });

        if (alreadyAddedLevels > 0)
        {
            this.AddErrorNotification
            (
                "Failed to add levels to playlist", 
                $"Failed to add {alreadyAddedLevels} out of {levelIdsCount} levels to playlist '{parent.Name}' because "+
                $"they already are in the playlist.",
                user
            );
        }

        if (nonExistentLevels > 0)
        {
            this.AddErrorNotification
            (
                "Failed to add levels to playlist", 
                $"Failed to add {nonExistentLevels} out of {levelIdsCount} levels to playlist '{parent.Name}' because "+
                $"they couldn't be found on the server.",
                user
            );
        }
    }

    public void UpdatePlaylistLevelOrder(GamePlaylist parent, List<int> levelIds, GameUser user)
    {
        IEnumerable<LevelPlaylistRelation> allRelations = this.GetLevelPlaylistRelationsForPlaylist(parent);

        // Get the relations for levels not included in the level ID list to adjust their indices
        IEnumerable<LevelPlaylistRelation> excludedRelations = allRelations.Where(r => !levelIds.Contains(r.Level.LevelId));
            
        int levelIdsCount = levelIds.Count;
        int newIndex = 0;
        int failedUpdates = 0;
        this.Write(() => 
        {
            // Iterate through the level IDs unlike with excludedRelations below, 
            // to preserve the reordered levels' order for the new relations' indices
            foreach (int levelId in levelIds)
            {
                LevelPlaylistRelation? relation = allRelations.FirstOrDefault(r => r.Level.LevelId == levelId);
                // If no relation exists for this level ID, skip it
                if (relation == null)
                {
                    failedUpdates++;
                }
                else
                {
                    this.OverwriteIndexAndIncrement(relation.Index, newIndex);
                }
            }

            // Now "append" the excluded relations by setting their indices greater than those of the newly ordered ones,
            // while preserving the excluded levels' previous custom order, which is possible thanks to
            // ordering the relations returned by GetLevelPlaylistRelationsForPlaylist() by index
            foreach (LevelPlaylistRelation relation in excludedRelations)
            {
                this.OverwriteIndexAndIncrement(relation.Index, newIndex);
            }

            // The parent playlist has been updated
            parent.LastUpdateDate = this._time.Now;
        });

        if (failedUpdates > 0)
        {
            this.AddErrorNotification
            (
                "Failed to reorder playlist levels", 
                $"Reordering {failedUpdates} out of {levelIdsCount} levels in playlist '{parent.Name}' "+
                $"failed due to them not being in the playlist.",
                user
            );
        }
    }

    /// <remarks>
    /// This method assumes it is called inside a write operation.
    /// </remarks>
    private void OverwriteIndexAndIncrement(int indexToOverwrite, int newIndex)
    {
        indexToOverwrite = newIndex;
        newIndex++;
    }

    private IEnumerable<LevelPlaylistRelation> GetLevelPlaylistRelationsForPlaylist(GamePlaylist parent)
        => this.LevelPlaylistRelations
            .Where(r => r.Playlist == parent)
            .OrderBy(r => r.Index);

    /// <remarks>
    /// A public IEnumerable method for this is nessesary in order to be able to detect (and remove) 
    /// loops in playlists using <see cref='GamePlaylistExtensions'/>
    /// </remarks>
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

    public DatabaseList<GameLevel> GetLevelsInPlaylist(GamePlaylist playlist, TokenGame game, int skip, int count)
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance. 
        => new(this.LevelPlaylistRelations
            .Where(l => l.Playlist == playlist)
            .OrderBy(r => r.Index)
            .AsEnumerable()
            .Select(l => l.Level)
            .FilterByGameVersion(game), skip, count);

    public DatabaseList<GameLevel> GetLevelsInPlaylist(GamePlaylist playlist, int skip, int count)
        // TODO: When we have postgres, remove the `AsEnumerable` call for performance. 
        => new(this.LevelPlaylistRelations
            .Where(l => l.Playlist == playlist)
            .OrderBy(r => r.Index)
            .AsEnumerable()
            .Select(l => l.Level), skip, count);

    public int GetTotalLevelsInPlaylistCount(GamePlaylist playlist, TokenGame game)
        => this.LevelPlaylistRelations.Where(l => l.Playlist == playlist)
            .AsEnumerable()
            .Select(l => l.Level)
            .FilterByGameVersion(game)
            .Count();

    public int GetTotalLevelsInPlaylistCount(GamePlaylist playlist)
        => this.LevelPlaylistRelations.Count(l => l.Playlist == playlist);

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