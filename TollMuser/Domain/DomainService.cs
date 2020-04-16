using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tolltech.Muser.Models;
using Tolltech.YandexClient;
using Tolltech.YandexClient.ApiModels;

namespace Tolltech.Muser.Domain
{
    public class DomainService : IDomainService
    {
        private readonly IYandexService yandexService;
        private readonly IVkService vkService;

        public DomainService(IYandexService yandexService, IVkService vkService)
        {
            this.yandexService = yandexService;
            this.vkService = vkService;
        }

        public async Task<VkTrack[]> GetNewVkTracksUnauthorizedAsync(string yaPlaylistId, Guid? userId, string vkUserId)
        {
            var yandexApi = await yandexService.GetClientAsync(userId).ConfigureAwait(false);
            var vkTracks = await vkService.GetVkTracksUnauthorizedAsync(vkUserId).ConfigureAwait(false);
            var yaTracks = await yandexApi.GetTracksAsync(yaPlaylistId).ConfigureAwait(false);

            //log.Info($"Found {yaTracks.Length} yaTracks from playlist {yaPlaylistId} and {vkTracks.Length} vkTracks");

            return new SyncTracks(vkTracks, yaTracks).GetNewTracks();
        }

        public async Task ImportTracksAsync(VkTrack[] trackToImport, string playlistId, Guid? userId,
            Action<(int Processed, int Total)> percentsComplete = null)
        {
            var yandexApi = await yandexService.GetClientAsync(userId).ConfigureAwait(false);

            var existentTracks = await yandexApi.GetTracksAsync(playlistId).ConfigureAwait(false);
            var existentTracksHash = existentTracks.Select(x => (Id: x.Id, AlbumId: x.Albums.FirstOrDefault()?.Id));
            var foundTracks = new HashSet<(string Id, string AlbumId)>(existentTracksHash);

            var completeCount = 0;
            var notFoundCount = 0;
            var totalCount = trackToImport.Length;

            //log.Info($"Start import {totalCount} tracks for user {userId}");

            foreach (var track in trackToImport)
            {
                try
                {
                    //log.Info($"START process {track.Artist} - {track.Title}");

                    var titleNormalizeForYandex = track.Title.NormalizeForYandex();
                    var artistNormalizeForYandex = track.Artist.NormalizeForYandex();
                    var yandexApiTracks = await yandexApi.SearchAsync($"{titleNormalizeForYandex} {artistNormalizeForYandex}").ConfigureAwait(false);
                    if (yandexApiTracks.Length == 0)
                    {
                        var normalizeTitle = titleNormalizeForYandex.NormalizeTrackInfo();
                        var normalizeArtist = artistNormalizeForYandex.NormalizeTrackInfo();
                        //log.Info($"TRY found normalized {normalizeArtist} - {normalizeTitle}");
                        yandexApiTracks = await yandexApi.SearchAsync($"{normalizeTitle} {normalizeArtist}").ConfigureAwait(false);
                    }

                    var yaTracks = new YandexTracks(yandexApiTracks);

                    var yaTrack = yaTracks.FindEqualTrack(track);

                    if (yaTrack == null)
                    {
                        ++notFoundCount;

                        //log.Info($"SKIP Not found {track.Artist} - {track.Title} Notfound {notFoundCount}");
                        var foundedYaTrack = yandexApiTracks.FirstOrDefault();
                        var artistStr = string.Join(", ", foundedYaTrack?.Artists.Select(x => x.Name) ?? Enumerable.Empty<string>());
                        //log.Info($"BUT found {artistStr} - {foundedYaTrack?.Title}\r\n{track.Artist}---{track.Title};{artistStr}---{foundedYaTrack?.Title}");
                        continue;
                    }

                    var trackHash = (Id: yaTrack.Id, AlbumId: yaTrack.Albums.First().Id);
                    if (foundTracks.Contains(trackHash))
                    {
                        //log.Info($"SKIP Already exists {track.Artist} - {track.Title} ({completeCount}/{totalCount})");
                        continue;
                    }
                    else
                    {
                        foundTracks.Add(trackHash);
                    }

                    var playlists = await yandexApi.GetPlaylistsAsync().ConfigureAwait(false);
                    var revision = playlists.FirstOrDefault(x => x.Id == playlistId)?.Revision;

                    var trackToChange = new TrackToChange {Id = trackHash.Id, AlbumId = trackHash.AlbumId};
                    await yandexApi.AddTracksToPlaylistAsync(playlistId, revision, trackToChange).ConfigureAwait(false);

                    await Task.Delay(200).ConfigureAwait(false);
                }
                catch (YandexApiException ex)
                {
                    //log.Info($"ERROR YandexApiError {ex.Message}");
                }
                finally
                {
                    percentsComplete?.Invoke((++completeCount, totalCount));
                    //log.Info($"PROCESSED {completeCount}/{totalCount} tracks. NotFound {notFoundCount}");
                }
            }
        }
    }
}