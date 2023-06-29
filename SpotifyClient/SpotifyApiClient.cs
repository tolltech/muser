using System.Net;
using System.Threading.Tasks;
using Tolltech.Serialization;
using Tolltech.SpotifyClient.ApiModels;

namespace Tolltech.SpotifyClient
{
    public class SpotifyApiClient : ISpotifyApiClient
    {
        private readonly string accessToken;
        private readonly IJsonSerializer serializer;

        public SpotifyApiClient(string accessToken, IJsonSerializer serializer)
        {
            this.accessToken = accessToken;
            this.serializer = serializer;
        }
        
        public async Task<ArtistInfo> GetArtist(string artistId)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Set("Authorization", $@"Bearer  {accessToken}");
                var response = await webClient
                    .DownloadDataTaskAsync($@"https://api.spotify.com/v1/artists/{artistId}")
                    .ConfigureAwait(false);

                return serializer.Deserialize<ArtistInfo>(response);
            }
        }

        public Task<Playlist[]> GetPlaylistsAsync(string userId = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<Track[]> GetTracksAsync(string playlistId)
        {
            throw new System.NotImplementedException();
        }

        public Task AddTracksToPlaylistAsync(string playlistId, string playlistRevision, params TrackToChange[] tracks)
        {
            throw new System.NotImplementedException();
        }

        public Task<Track[]> SearchAsync(string query)
        {
            throw new System.NotImplementedException();
        }
    }
}