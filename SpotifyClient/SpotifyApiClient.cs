using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MusicClientCore;
using Newtonsoft.Json;
using Tolltech.Serialization;
using Tolltech.SpotifyClient.ApiModels;
using Array = System.Array;

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

        private async Task<T> DoGet<T>(string uri, object param = null)
        {
            using (var webClient = new WebClient())
            {
                var formed = param == null ? uri : $"{uri}?{param.ToFormDataStr()}";

                webClient.Headers.Set("Authorization", $@"Bearer  {accessToken}");
                var response = await webClient
                    .DownloadDataTaskAsync($@"https://api.spotify.com/v1/{formed}")
                    .ConfigureAwait(false);

                return serializer.Deserialize<T>(response);
            }
        }

        private async Task<TResponse> DoPost<TResponse, TBody>(string uri, TBody body = null, object param = null) where TBody : class
        {
            using (var webClient = new WebClient())
            {
                var formed = param == null ? uri : $"{uri}?{param.ToFormDataStr()}";

                webClient.Headers.Set("Authorization", $@"Bearer  {accessToken}");
                webClient.Headers.Set("Content-Type", $@"application/json");
                var response = await webClient
                    .UploadDataTaskAsync($@"https://api.spotify.com/v1/{formed}", serializer.Serialize(body))
                    .ConfigureAwait(false);

                return serializer.Deserialize<TResponse>(response);
            }
        }
        
        private async Task<TResponse> DoDelete<TResponse, TBody>(string uri, TBody body = null, object param = null) where TBody : class
        {
            var formed = param == null ? uri : $"{uri}?{param.ToFormDataStr()}";
            using(var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete,  $@"https://api.spotify.com/v1/{formed}"))
            {
                httpRequestMessage.Headers.Add("Authorization", $@"Bearer  {accessToken}");

                httpRequestMessage.Content =
                    new StringContent(serializer.SerializeToString(body), Encoding.UTF8, "application/json");

                var response = await client
                    .SendAsync(httpRequestMessage)
                    .ConfigureAwait(false);

                var responseStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return serializer.DeserializeFromString<TResponse>(responseStr);
            }
        }

        class ItemsResponse<T>
        {
            public ItemsResponse()
            {
                Items = Array.Empty<T>();
            }

            [JsonProperty("items")] public T[] Items { get; set; }
        }

        public async Task<Playlist[]> GetPlaylistsAsync()
        {
            return (await DoGet<ItemsResponse<Playlist>>(@"me/playlists").ConfigureAwait(false)).Items;
        }

        class TrackWrapper
        {
            [JsonProperty("track")] public Track Track { get; set; }
        }

        public async Task<Track[]> GetTracksAsync(string playlistId)
        {
            var result = new List<Track>();
            var offset = 0;
            do
            {
                var param = new
                {
                    limit = 50,
                    offset = offset,
                    //fields = "track(id,name,album(name,id), artists(name,id))"
                };
                var page = await DoGet<ItemsResponse<TrackWrapper>>($@"playlists/{playlistId}/tracks", param)
                    .ConfigureAwait(false);
                result.AddRange(page.Items.Select(x => x.Track).ToArray());

                if (page.Items.Length == 0) break;

                offset += page.Items.Length;
            } while (true);

            return result.ToArray();
        }

        class AddTracksBody
        {
            [JsonProperty("uris")] public string[] Uris { get; set; }
            [JsonProperty("position")] public int Position { get; set; }
        }

        class AddTracksResponse
        {
            [JsonProperty("snapshot_id")] public string Revision { get; set; }
        }
        
        public async Task AddTracksToPlaylistAsync(string playlistId, string playlistRevision, params TrackToChange[] tracks)
        {
            var offset = 0;
            do
            {
                var page = tracks.Skip(offset).Take(100).ToArray();

                if (page.Length == 0) break;

                var body = new AddTracksBody
                {
                    Position = offset,
                    Uris = tracks.Select(x=> $"spotify:track:{x.Id}").ToArray()
                };
                await DoPost<AddTracksResponse, AddTracksBody>($@"playlists/{playlistId}/tracks", body).ConfigureAwait(false);

                offset += page.Length;
            } while (true);
        }

        class TracksWrapper
        {
            [JsonProperty("tracks")] public ItemsResponse<Track> Tracks { get; set; }
        }
        public async Task<Track[]> SearchAsync(string query)
        {
            var o = new
            {
                q = query, 
                type = "track"
            };
            return (await DoGet<TracksWrapper>("search", o).ConfigureAwait(false)).Tracks.Items;
        }

        class RemoveTrack
        {
            [JsonProperty("uri")] public string Uri { get; set; }
        }
        
        class RemoveTracksBody
        {
            [JsonProperty("tracks")] public RemoveTrack[] Tracks { get; set; }
            [JsonProperty("snapshot_id")] public string Revision { get; set; }
        }
        
        static readonly HttpClient client = new HttpClient();
        public async Task RemoveTracksToPlaylistAsync(string playlistId, string revision, TrackToChange[] trackToChanges)
        {
            var offset = 0;
            do
            {
                var page = trackToChanges.Skip(offset).Take(100).ToArray();

                if (page.Length == 0) break;

                var body = new RemoveTracksBody
                {
                    Revision = revision,
                    Tracks = trackToChanges.Select(x => new RemoveTrack { Uri = $"spotify:track:{x.Id}" }).ToArray()
                };

                await DoDelete<AddTracksResponse, RemoveTracksBody>($@"playlists/{playlistId}/tracks", body).ConfigureAwait(false);

                offset += page.Length;
            } while (true);
        }
    }
}