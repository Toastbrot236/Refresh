using Refresh.Database;
using Refresh.Database.Models.Authentication;
using Refresh.Database.Models.Levels;
using Refresh.Database.Models.Playlists;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.Game.Endpoints.DataTypes.Response;
using Refresh.Interfaces.Game.Types.Playlists;
using RefreshTests.GameServer.Extensions;

namespace RefreshTests.GameServer.Tests.Playlists;

public class Lbp1PlaylistOperationTests : GameServerTest
{
    [Test]
    public void CreateLbp1Playlist()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet1, TokenPlatform.PS3, user);

        // Create the root playlist
        HttpResponseMessage message = client.PostAsync($"/lbp/createPlaylist", new()).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        SerializedLbp1Playlist rootPlaylist = message.Content.ReadAsXML<SerializedLbp1Playlist>();
        Assert.That(rootPlaylist.Id, Is.EqualTo(1));

        // Check that the user has that root playlist's ID in their user response
        message = client.GetAsync($"/lbp/user/{user.Username}").Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        GameUserResponse result = message.Content.ReadAsXML<GameUserResponse>();
        Assert.That(result.RootPlaylistId, Is.EqualTo("1"));

        // Now create the first actual playlist
        message = client.PostAsync($"/lbp/createPlaylist", null).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        SerializedLbp1Playlist playlist1Response = message.Content.ReadAsXML<SerializedLbp1Playlist>();
        Assert.That(playlist1Response.Id, Is.EqualTo(2));

        // Check whether it actually exists
        Assert.That(context.Database.GetPlaylistById(2), Is.Not.Null);

        // Create a sub-playlist for that playlist
        message = client.PostAsync($"/lbp/createPlaylist?parent_id=2", null).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        SerializedLbp1Playlist playlist2Response = message.Content.ReadAsXML<SerializedLbp1Playlist>();
        Assert.That(playlist2Response.Id, Is.EqualTo(3));

        // Check whether it actually exists
        GamePlaylist? playlist2 = context.Database.GetPlaylistById(3);
        Assert.That(playlist2, Is.Not.Null);

        // Make sure the sub-playlist was added to its parent playlist
        DatabaseList<GamePlaylist> subPlaylists = context.Database.GetPlaylistsInPlaylist(playlist2!, 0, 10);
        Assert.That(subPlaylists.Items.Count, Is.EqualTo(1));
        Assert.That(subPlaylists.TotalItems, Is.EqualTo(1));
        Assert.That(subPlaylists.Items.First().PlaylistId, Is.EqualTo(3));
    }

    [Test]
    public void AddPlaylistToPlaylist()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet1, TokenPlatform.PS3, user);

        // Create playlists
        GamePlaylist playlistToAddTo = context.CreatePlaylist(user);
        GamePlaylist playlistToAdd = context.CreatePlaylist(user);
        
        // None of the playlists has each other yet
        Assert.That(context.Database.GetPlaylistsInPlaylist(playlistToAddTo, 0, 10).TotalItems, Is.Zero);
        Assert.That(context.Database.GetPlaylistsInPlaylist(playlistToAdd, 0, 10).TotalItems, Is.Zero);

        // Now add
        HttpResponseMessage message = client.PostAsync($"/lbp/addToPlaylist/{playlistToAddTo.PlaylistId}?slot_type=playlist&slot_id={playlistToAdd.PlaylistId}", null).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        // Does the parent playlist now have a sub-playlist?
        DatabaseList<GamePlaylist> subPlaylists = context.Database.GetPlaylistsInPlaylist(playlistToAddTo, 0, 10);
        Assert.That(subPlaylists.Items.Count, Is.EqualTo(1));
        Assert.That(subPlaylists.TotalItems, Is.EqualTo(1));
        Assert.That(subPlaylists.Items.First().PlaylistId, Is.EqualTo(playlistToAdd.PlaylistId));
    }

    [Test]
    public void DontAddPlaylistToItself()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet1, TokenPlatform.PS3, user);

        // Create playlist and add it to itself
        GamePlaylist playlist = context.CreatePlaylist(user);

        HttpResponseMessage message = client.PostAsync($"/lbp/addToPlaylist/{playlist.PlaylistId}?slot_type=playlist&slot_id={playlist.PlaylistId}", null).Result;
        Assert.That(message.StatusCode, Is.EqualTo(BadRequest));

        // Make sure the server actually prevented this
        DatabaseList<GamePlaylist> subPlaylists = context.Database.GetPlaylistsInPlaylist(playlist, 0, 10);
        Assert.That(subPlaylists.Items.Count, Is.EqualTo(0));
        Assert.That(subPlaylists.TotalItems, Is.EqualTo(0));
    }

    [Test]
    public void DontAddPlaylistToItsChild()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet1, TokenPlatform.PS3, user);

        // Create playlists and add one to another
        GamePlaylist playlist = context.CreatePlaylist(user);
        GamePlaylist child = context.CreatePlaylist(user);

        HttpResponseMessage message = client.PostAsync($"/lbp/addToPlaylist/{child.PlaylistId}?slot_type=playlist&slot_id={playlist.PlaylistId}", null).Result;
        Assert.That(message.StatusCode, Is.EqualTo(BadRequest));

        // Make sure the server actually prevented this
        DatabaseList<GamePlaylist> subPlaylists = context.Database.GetPlaylistsInPlaylist(playlist, 0, 10);
        Assert.That(subPlaylists.Items.Count, Is.EqualTo(0));
        Assert.That(subPlaylists.TotalItems, Is.EqualTo(0));

        // Make sure it didn't go the other way either
        subPlaylists = context.Database.GetPlaylistsInPlaylist(child, 0, 10);
        Assert.That(subPlaylists.Items.Count, Is.EqualTo(0));
        Assert.That(subPlaylists.TotalItems, Is.EqualTo(0));
    }

    [Test]
    public void RemovePlaylistFromPlaylist()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet1, TokenPlatform.PS3, user);

        // Create playlists and add one to another
        GamePlaylist playlistToRemoveFrom = context.CreatePlaylist(user);
        GamePlaylist playlistToRemove = context.CreatePlaylist(user);
        context.Database.AddPlaylistToPlaylist(playlistToRemove, playlistToRemoveFrom);

        // Ensure it was actually added
        DatabaseList<GamePlaylist> subPlaylists = context.Database.GetPlaylistsInPlaylist(playlistToRemoveFrom, 0, 10);
        Assert.That(subPlaylists.Items.Count, Is.EqualTo(1));
        Assert.That(subPlaylists.TotalItems, Is.EqualTo(1));
        Assert.That(subPlaylists.Items.First().PlaylistId, Is.EqualTo(playlistToRemove.PlaylistId));

        // Now remove
        HttpResponseMessage message = client.PostAsync($"/lbp/removeFromPlaylist/{playlistToRemoveFrom.PlaylistId}?slot_type=playlist&slot_id={playlistToRemove.PlaylistId}", null).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        // None of the non-root playlists has each other anymore
        subPlaylists = context.Database.GetPlaylistsInPlaylist(playlistToRemove, 0, 10);
        Assert.That(subPlaylists.Items.Count, Is.EqualTo(0));
        Assert.That(subPlaylists.TotalItems, Is.EqualTo(0));

        subPlaylists = context.Database.GetPlaylistsInPlaylist(playlistToRemoveFrom, 0, 10);
        Assert.That(subPlaylists.Items.Count, Is.EqualTo(0));
        Assert.That(subPlaylists.TotalItems, Is.EqualTo(0));
    }

    [Test]
    public void AddLevelToPlaylist()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet1, TokenPlatform.PS3, user);

        GamePlaylist playlist = context.CreatePlaylist(user);
        GameLevel level = context.CreateLevel(user);

        // Now add
        HttpResponseMessage message = client.PostAsync($"/lbp/addToPlaylist/{playlist.PlaylistId}?slot_type=user&slot_id={level.LevelId}", null).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        // Does the parent playlist now have a level?
        DatabaseList<GameLevel> subLevels = context.Database.GetLevelsInPlaylist(playlist, TokenGame.LittleBigPlanet1, 0, 10);
        Assert.That(subLevels.Items.Count, Is.EqualTo(1));
        Assert.That(subLevels.TotalItems, Is.EqualTo(1));
        Assert.That(subLevels.Items.First().LevelId, Is.EqualTo(level.LevelId));
    }

    [Test]
    public void RemoveLevelFromPlaylist()
    {
        using TestContext context = this.GetServer();
        GameUser user = context.CreateUser();
        using HttpClient client = context.GetAuthenticatedClient(TokenType.Game, TokenGame.LittleBigPlanet1, TokenPlatform.PS3, user);

        GamePlaylist playlist = context.CreatePlaylist(user);
        GameLevel level = context.CreateLevel(user);

        // Add level to playlist
        context.Database.AddLevelToPlaylist(level, playlist);

        // Ensure it was actually added
        DatabaseList<GameLevel> subLevels = context.Database.GetLevelsInPlaylist(playlist, TokenGame.LittleBigPlanet1, 0, 10);
        Assert.That(subLevels.Items.Count, Is.EqualTo(1));
        Assert.That(subLevels.TotalItems, Is.EqualTo(1));
        Assert.That(subLevels.Items.First().LevelId, Is.EqualTo(level.LevelId));

        // Now remove
        HttpResponseMessage message = client.PostAsync($"/lbp/removeFromPlaylist/{playlist.PlaylistId}?slot_type=user&slot_id={level.LevelId}", null).Result;
        Assert.That(message.StatusCode, Is.EqualTo(OK));

        // There are no levels in the playlist anymore
        subLevels = context.Database.GetLevelsInPlaylist(playlist, TokenGame.LittleBigPlanet1, 0, 10);
        Assert.That(subLevels.Items.Count, Is.EqualTo(0));
        Assert.That(subLevels.TotalItems, Is.EqualTo(0));
    }
}