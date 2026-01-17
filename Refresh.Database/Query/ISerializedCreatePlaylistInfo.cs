using Refresh.Database.Models;

namespace Refresh.Database.Query;

public interface ISerializedCreatePlaylistInfo
{
    string? Name { get; }
    string? Description { get; }
    string? IconHash { get; }
    GameLocation? Location { get; }
}