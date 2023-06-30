using System.Threading.Tasks;
using JetBrains.Annotations;
using Tolltech.SpotifyClient.ApiModels;

namespace Tolltech.SpotifyClient
{
    public interface ISpotifyApiClient
    {
        Task<ArtistInfo> GetArtist(string artistId);

        [ItemNotNull]
        [NotNull]
        Task<Playlist[]> GetPlaylistsAsync();
        
        [ItemNotNull]
        [NotNull]
        Task<Track[]> GetTracksAsync(string playlistId);
        
        [NotNull]
        Task AddTracksToPlaylistAsync(string playlistId, string playlistRevision, params TrackToChange[] tracks);
        
        [ItemNotNull]
        [NotNull]
        Task<Track[]> SearchAsync(string query);

        Task RemoveTracksToPlaylistAsync(string playlistId, string revision, TrackToChange[] trackToChanges);
    }
}