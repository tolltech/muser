using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Tolltech.Muser.Models;
using Tolltech.YandexClient;
using Tolltech.YandexClient.ApiModels;

namespace Tolltech.Muser.Domain
{
    public class DomainService : IDomainService
    {
        private readonly IYandexService yandexService;
        private readonly INormalizedTrackService normalizedTrackService;
        private static readonly ILog log = LogManager.GetLogger(typeof(DomainService));

        public DomainService(IYandexService yandexService, INormalizedTrackService normalizedTrackService)
        {
            this.yandexService = yandexService;
            this.normalizedTrackService = normalizedTrackService;
        }

        public async Task<NormalizedTrack[]> GetNewTracksAsync(string yaPlaylistId, Guid? userId,
            SourceTrack[] inputTracks)
        {
            var yandexApi = await yandexService.GetClientAsync(userId).ConfigureAwait(false);
            var normalizedTracks = normalizedTrackService.GetNormalizedTracks(inputTracks);
            var yaTracks = await yandexApi.GetTracksAsync(yaPlaylistId).ConfigureAwait(false);

            log.Info(
                $"Found {yaTracks.Length} yaTracks from playlist {yaPlaylistId} and {normalizedTracks.Length} vkTracks");

            return new SyncTracks(normalizedTracks, yaTracks).GetNewTracks();
        }

        public async Task<ImportResult[]> ImportTracksAsync(NormalizedTrack[] trackToImport, string playlistId,
            Guid? userId,
            Action<(int Processed, int Total, ImportResult importResult)> percentsComplete = null)
        {
            var yandexApi = await yandexService.GetClientAsync(userId).ConfigureAwait(false);
            var existentTracks = await yandexApi.GetTracksAsync(playlistId).ConfigureAwait(false);

            var existentTracksHash = existentTracks.Select(x => (Id: x.Id, AlbumId: x.Albums.FirstOrDefault()?.Id));
            var foundTracks = new HashSet<(string Id, string AlbumId)>(existentTracksHash);

            var totalCount = trackToImport.Length;

            log.Info($"Start import {totalCount} tracks for user {userId}");

            var syncTracks = new SyncTracks(trackToImport, existentTracks);
            var newTracks = syncTracks.GetNewTracks();

            var alreadyExistentTracks = trackToImport
                .Except(newTracks)
                .Select(x => new ImportResult(x.Artist, x.Title, playlistId)
                {
                    ImportStatus = ImportStatus.AlreadyExists,
                    Message = "This track should not be in this request",
                })
                .ToArray();

            var result = new List<ImportResult>(trackToImport.Length);
            result.AddRange(alreadyExistentTracks);
            var notFoundCount = 0;
            var completeCount = alreadyExistentTracks.Length;

            foreach (var track in newTracks)
            {
                log.Info($"START process {track.Artist} - {track.Title}");
                var importResult = new ImportResult(track.Artist, track.Title, playlistId);

                try
                {
                    var titleNormalizeForYandex = track.Title.NormalizeForYandex();
                    var artistNormalizeForYandex = track.Artist.NormalizeForYandex();
                    var yandexApiTracks = await yandexApi
                        .SearchAsync($"{titleNormalizeForYandex} {artistNormalizeForYandex}").ConfigureAwait(false);
                    if (yandexApiTracks.Length == 0)
                    {
                        var normalizeTitle = titleNormalizeForYandex.NormalizeTrackInfo();
                        var normalizeArtist = artistNormalizeForYandex.NormalizeTrackInfo();

                        importResult.NormalizedTrack = new ImportingTrack
                        {
                            Artist = normalizeArtist,
                            Title = normalizeTitle
                        };

                        log.Info($"TRY found normalized {normalizeArtist} - {normalizeTitle}");
                        yandexApiTracks = await yandexApi.SearchAsync($"{normalizeTitle} {normalizeArtist}")
                            .ConfigureAwait(false);
                    }

                    var yaTracks = new YandexTracks(yandexApiTracks);

                    var yaTrack = yaTracks.FindEqualTrack(track);

                    if (yaTrack == null)
                    {
                        ++notFoundCount;

                        log.Info($"SKIP Not found {track.Artist} - {track.Title} Notfound {notFoundCount}");
                        var foundedYaTrack = yandexApiTracks.FirstOrDefault();
                        var artistStr = foundedYaTrack?.ArtistsStr;

                        importResult.ImportStatus = ImportStatus.NotFound;
                        importResult.Candidate = new ImportingTrack
                        {
                            Artist = artistStr,
                            Title = foundedYaTrack?.Title
                        };

                        importResult.CandidateTrackId = foundedYaTrack?.Id;
                        importResult.CandidateAlbumId = foundedYaTrack?.Albums?.FirstOrDefault()?.Id;

                        log.Info(
                            $"BUT found {artistStr} - {foundedYaTrack?.Title}\r\n{track.Artist}---{track.Title};{artistStr}---{foundedYaTrack?.Title}");

                        continue;
                    }

                    importResult.Candidate = new ImportingTrack
                    {
                        Artist = yaTrack.ArtistsStr,
                        Title = yaTrack.Title
                    };

                    importResult.CandidateTrackId = yaTrack.Id;
                    importResult.CandidateAlbumId = yaTrack.Albums?.FirstOrDefault()?.Id;

                    var trackHash = (Id: yaTrack.Id, AlbumId: yaTrack.Albums.First().Id);
                    if (foundTracks.Contains(trackHash))
                    {
                        importResult.ImportStatus = ImportStatus.AlreadyExists;
                        log.Info($"SKIP Already exists {track.Artist} - {track.Title} ({completeCount}/{totalCount})");
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

                    importResult.ImportStatus = ImportStatus.Ok;

                    await Task.Delay(200).ConfigureAwait(false);
                }
                catch (YandexApiException ex)
                {
                    log.Info($"ERROR YandexApiError {ex.Message}");
                    importResult.ImportStatus = ImportStatus.Error;
                    importResult.Message = $"Error: {ex.Message}. StackTrace: {ex.StackTrace}";
                }
                finally
                {
                    result.Add(importResult);
                    percentsComplete?.Invoke((++completeCount, totalCount, importResult));
                    log.Info($"PROCESSED {completeCount}/{totalCount} tracks. NotFound {notFoundCount}");
                }
            }

            return result.ToArray();
        }

        public async Task ImportTracksAsync(TrackToChange[] trackToImport, string playlistId, Guid? userId)
        {
            var yandexApi = await yandexService.GetClientAsync(userId).ConfigureAwait(false);

            var existentTracks = await yandexApi.GetTracksAsync(playlistId).ConfigureAwait(false);

            var existentTrackHashes = new HashSet<(string, string)>(existentTracks
                .SelectMany(track =>
                    track.Albums
                        .Select(album => (track.Id, album.Id))));

            var newTracks = trackToImport.Where(x => !existentTrackHashes.Contains((x.Id, x.AlbumId))).ToArray();

            var playlists = await yandexApi.GetPlaylistsAsync().ConfigureAwait(false);
            var revision = playlists.FirstOrDefault(x => x.Id == playlistId)?.Revision;
            await yandexApi.AddTracksToPlaylistAsync(playlistId, revision, newTracks).ConfigureAwait(false);
        }
    }
}