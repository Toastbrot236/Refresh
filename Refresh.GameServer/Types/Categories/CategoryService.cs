using System.Collections.Frozen;
using Bunkum.Core.Services;
using NotEnoughLogs;
using Refresh.GameServer.Types.Categories.Levels;
using Refresh.GameServer.Types.Categories.Playlists;
using Refresh.GameServer.Types.Categories.Users;

namespace Refresh.GameServer.Types.Categories;

public class CategoryService : EndpointService
{
    // Level Categories
    public readonly FrozenSet<LevelCategory> LevelCategories;

    // ReSharper disable once InconsistentNaming
    private readonly List<LevelCategory> _levelCategories =
    [
        new CoolLevelsCategory(),
        new TeamPickedLevelsCategory(),

        new CurrentlyPlayingCategory(),
        new RandomLevelsCategory(),
        new NewestLevelsCategory(),

        new MostHeartedLevelsCategory(),
        new HighestRatedLevelsCategory(),
        new MostUniquelyPlayedLevelsCategory(),
        new MostReplayedLevelsCategory(),

        new ByUserLevelCategory(),
        new FavouriteLevelsByUserCategory(),
        new QueuedLevelsByUserCategory(),

        new SearchLevelCategory(),
        new ByTagCategory(),
        new DeveloperLevelsCategory(),
        new ContestCategory(),
        new AdventureCategory(),
    ];

    // Playlist Categories
    public readonly FrozenSet<PlaylistCategory> PlaylistCategories;

    // ReSharper disable once InconsistentNaming
    private readonly List<PlaylistCategory> _playlistCategories =
    [
        
        new FavouritePlaylistsByUserCategory(),
    ];

    // User Categories
    public readonly FrozenSet<UserCategory> UserCategories;

    // ReSharper disable once InconsistentNaming
    private readonly List<UserCategory> _userCategories =
    [

    ];


    internal CategoryService(Logger logger) : base(logger)
    {
        this.LevelCategories = this._levelCategories.ToFrozenSet();
        this.PlaylistCategories = this._playlistCategories.ToFrozenSet();
        this.UserCategories = this._userCategories.ToFrozenSet();
    }
}