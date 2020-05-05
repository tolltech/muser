using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tolltech.Serialization;
using Tolltech.YandexClient.ApiModels;
using Tolltech.YandexClient.Authorizations;

namespace Tolltech.YandexClient
{
    public class YandexMusicClient : IYandexMusicClient
    {
        private readonly IYandexCredentials yandexCredentials;
        private readonly IJsonSerializer serializer;
        private const string urlPrefix = "https://api.music.yandex.net";

        public YandexMusicClient(IYandexCredentials yandexCredentials, IJsonSerializer serializer)
        {
            this.yandexCredentials = yandexCredentials;
            this.serializer = serializer;
        }

        public string Login => yandexCredentials.Login;

        public async Task<Playlist[]> GetPlaylistsAsync(string userId)
        {
            var authorizationInfo = await yandexCredentials.GetAuthorizationInfoAsync().ConfigureAwait(false);
            var realUserId = userId ?? authorizationInfo.Uid;

            var result = await DoRequestAsync<Playlist[]>($"users/{realUserId}/playlists/list")
                .ConfigureAwait(false);
            return result.Resilt;
        }

        public async Task<Playlist> CreatePlaylistAsync(string title)
        {
            var authInfo = await yandexCredentials.GetAuthorizationInfoAsync().ConfigureAwait(false);
            var body = new
            {
                title = title,
                visibility = "private"
            };

            var url = $"/users/{authInfo.Uid}/playlists/create?{body.ToFormDataStr()}";
            return (await DoRequestAsync<Playlist>(url, HttpMethod.Post, body)
                .ConfigureAwait(false)).Resilt;
        }

        public async Task DeletePlaylistAsync(string id)
        {
            var authInfo = await yandexCredentials.GetAuthorizationInfoAsync().ConfigureAwait(false);
            await DoRequestAsync<object>($"/users/{authInfo.Uid}/playlists/{id}/delete", HttpMethod.Post)
                .ConfigureAwait(false);
        }

        public async Task<Track[]> GetTracksAsync(string playlistId)
        {
            var authInfo = await yandexCredentials.GetAuthorizationInfoAsync().ConfigureAwait(false);
            var parameters = new Dictionary<string, string>
            {
                {"kinds", $"{playlistId}"},
                {"mixed", bool.FalseString.ToLower()},
                {"rich-tracks", bool.TrueString.ToLower()}
            };
            return (await DoRequestAsync<Playlist[]>($"users/{authInfo.Uid}/playlists?{parameters.ToUriParams()}")
                       .ConfigureAwait(false)).Resilt.FirstOrDefault()?.Tracks.Select(x => x.Track).ToArray() ??
                   Array.Empty<Track>();
        }

        public async Task AddTracksToPlaylistAsync(string playlistId, string playlistRevision, params TrackToChange[] tracks)
        {
            var authInfo = await yandexCredentials.GetAuthorizationInfoAsync().ConfigureAwait(false);
            var body = new
            {
                diff = serializer.SerializeToString(new[]
                {
                    new
                    {
                        op = "insert",
                        at = 0,
                        tracks = tracks
                    }
                }),
                revision = playlistRevision
            };

            var url = $"/users/{authInfo.Uid}/playlists/{playlistId}/change-relative?{body.ToFormDataStr()}";
            await DoRequestAsync<object>(url, HttpMethod.Post).ConfigureAwait(false);
        }

        public async Task RemoveTracksToPlaylistAsync(string playlistId, string playlistRevision,
            TrackToChange[] tracks)
        {
            var authInfo = await yandexCredentials.GetAuthorizationInfoAsync().ConfigureAwait(false);
            var body = new
            {
                diff = serializer.SerializeToString(new[]
                {
                    new
                    {
                        op = "delete",
                        from = 0,
                        to = tracks.Length,
                        tracks = tracks
                    }
                }),
                revision = playlistRevision
            };

            var url = $"/users/{authInfo.Uid}/playlists/{playlistId}/change-relative?{body.ToFormDataStr()}";
            await DoRequestAsync<object>(url, HttpMethod.Post).ConfigureAwait(false);
        }

        public async Task<Track[]> SearchAsync(string query)
        {
            var parameters = new Dictionary<string, string>
            {
                {"type", $"track"},
                {"text", query},
                {"page", "0"},
                {"nococrrect", bool.FalseString.ToLower()},
            };
            return (await DoRequestAsync<TrackSearchApiResult>($"search?{parameters.ToUriParams()}")
                       .ConfigureAwait(false)).Resilt?.Tracks?.Tracks ?? Array.Empty<Track>();
        }

        private async Task<ApiResult<T>> DoRequestAsync<T>(string url, HttpMethod httpMethod = HttpMethod.Get, object body = null)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var authInfo = await yandexCredentials.GetAuthorizationInfoAsync().ConfigureAwait(false);
                    webClient.Headers.Set("Authorization", $"OAuth {authInfo.Token}");

                    var fullUrl = $"{urlPrefix}/{url}";
                    var firstResponse = httpMethod == HttpMethod.Post
                        ? await webClient.UploadDataTaskAsync(fullUrl, body.ToFormData()).ConfigureAwait(false)
                        : await webClient.DownloadDataTaskAsync(fullUrl).ConfigureAwait(false);

                    var f = Encoding.UTF8.GetString(firstResponse);

                    return serializer.Deserialize<ApiResult<T>>(firstResponse);
                }
            }
            catch (WebException e)
            {
                var responseStream = e.Response?.GetResponseStream();
                if (responseStream == null)
                {
                    throw new YandexApiException($"YandexApi Error {e.Status} {e.Message}");
                }

                using (var reader = new StreamReader(responseStream))
                {
                    throw new YandexApiException($"YandexApi Error {await reader.ReadToEndAsync().ConfigureAwait(false)}");
                }
            }
        }

        private enum HttpMethod
        {
            Get = 1,
            Post = 2
        }
    }
}