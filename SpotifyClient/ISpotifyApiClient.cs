using System.Threading.Tasks;

namespace Tolltech.SpotifyClient
{
    public interface ISpotifyApiClient
    {
        Task<ArtistInfo> GetArtist(string artistId);

        // [ItemNotNull]
        // [NotNull]
        // Task<Playlist[]> GetPlaylistsAsync([CanBeNull] string userId = null);
        //
        // [ItemNotNull]
        // [NotNull]
        // Task<Playlist> CreatePlaylistAsync([NotNull] string title);
        //
        // [NotNull]
        // Task DeletePlaylistAsync([NotNull] string id);
        //
        // [ItemNotNull]
        // [NotNull]
        // Task<Track[]> GetTracksAsync(string playlistId);
        //
        // [NotNull]
        // Task AddTracksToPlaylistAsync(string playlistId, string playlistRevision, params TrackToChange[] tracks);
        //
        // [NotNull]
        // Task RemoveTracksToPlaylistAsync(string playlistId, string playlistRevision, TrackToChange[] tracks);
        //
        // [ItemNotNull]
        // [NotNull]
        // Task<Track[]> SearchAsync(string query);
    }
}