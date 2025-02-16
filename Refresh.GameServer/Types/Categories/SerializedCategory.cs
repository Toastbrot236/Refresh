using System.Xml.Serialization;

namespace Refresh.GameServer.Types.Categories;

#nullable disable

[XmlType("category")]
public class SerializedCategory
{
    [XmlElement("name")] public string Name { get; set; }
    [XmlElement("description")] public string Description { get; set; }
    [XmlElement("url")] public string Url { get; set; }
    [XmlElement("tag")] public string Tag { get; set; }
    [XmlElement("icon")] public string IconHash { get; set; }
    
    [XmlArray("types")] 
    [XmlArrayItem("type")] public string[] Types { get; set; }

    public static SerializedCategory FromCategory(Category category)
    {
        SerializedCategory serializedCategory = new()
        {
            Name = category.Name,
            Description = category.Description,
            Url = "/searches/" + category.ApiRoute,
            Tag = category.ApiRoute,
            IconHash = category.IconHash,
        };

        return serializedCategory;
    }

    /*
    public static SerializedLevelCategory FromCategory(LevelCategory levelCategory,
        RequestContext context,
        DataContext dataContext,
        int skip = 0,
        int count = 20)
    {
        SerializedLevelCategory serializedLevelCategory = (SerializedLevelCategory)FromCategory(levelCategory);

        LevelFilterSettings filterSettings = new(context, dataContext.Token!.TokenGame);
        DatabaseList<GameLevel> categoryLevels = levelCategory.Fetch(context, skip, count, dataContext, filterSettings, dataContext.User);
        
        IEnumerable<GameMinimalLevelResponse> levels = categoryLevels?.Items
            .Select(l => GameMinimalLevelResponse.FromOld(l, dataContext)) ?? [];

        serializedLevelCategory.Items = new SerializedMinimalLevelList(levels, categoryLevels?.TotalItems ?? 0, skip + count);

        return serializedLevelCategory;
    }

    public static SerializedPlaylistCategory FromCategory(PlaylistCategory playlistCategory,
        RequestContext context,
        DataContext dataContext,
        int skip = 0,
        int count = 20)
    {
        SerializedPlaylistCategory serializedPlaylistCategory = (SerializedPlaylistCategory)FromCategory(playlistCategory);

        LevelFilterSettings filterSettings = new(context, dataContext.Token!.TokenGame);
        DatabaseList<GamePlaylist> categoryPlaylists = playlistCategory.Fetch(context, skip, count, dataContext, filterSettings, dataContext.User);
        
        IEnumerable<SerializedLbp3Playlist> playlists = categoryPlaylists?.Items
            .Select(l => SerializedLbp3Playlist.FromOld(l, dataContext)) ?? [];

        serializedPlaylistCategory.Items = new SerializedLbp3PlaylistList(playlists, categoryPlaylists?.TotalItems ?? 0, skip + count);

        return serializedPlaylistCategory;
    }

    public static SerializedUserCategory FromCategory(UserCategory userCategory,
        RequestContext context,
        DataContext dataContext,
        int skip = 0,
        int count = 20)
    {
        SerializedUserCategory serializedUserCategory = (SerializedUserCategory)FromCategory(userCategory);

        LevelFilterSettings filterSettings = new(context, dataContext.Token!.TokenGame);
        DatabaseList<GameUser> categoryUsers = userCategory.Fetch(context, skip, count, dataContext, filterSettings, dataContext.User);
        
        IEnumerable<GameUserResponse> users = categoryUsers?.Items
            .Select(l => GameUserResponse.FromOld(l, dataContext)) ?? [];

        serializedUserCategory.Items = new SerializedUserList
        {
            Users = users.ToList(),
        };

        return serializedUserCategory;
    }

    */
}