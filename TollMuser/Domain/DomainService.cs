using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tolltech.Muser.Models;
using Tolltech.SpotifyClient;
using Tolltech.SpotifyClient.ApiModels;
using Vostok.Logging.Abstractions;

namespace Tolltech.Muser.Domain
{
    public class DomainService : IDomainService
    {
        private readonly IYandexService yandexService;
        private readonly INormalizedTrackService normalizedTrackService;
        private static readonly ILog log = LogProvider.Get();

        public DomainService(IYandexService yandexService, INormalizedTrackService normalizedTrackService)
        {
            this.yandexService = yandexService;
            this.normalizedTrackService = normalizedTrackService;
        }

        public async Task<NormalizedTrack[]> GetNewTracksAsync(string yaPlaylistId, Guid userId, SourceTrack[] inputTracks)
        {
            var yandexApi = yandexService.GetClientAsync(userId);
            var normalizedTracks = normalizedTrackService.GetNormalizedTracks(inputTracks);
            var yaTracks = await yandexApi.GetTracksAsync(yaPlaylistId).ConfigureAwait(false);

            log.Info(
                $"Found {yaTracks.Length} yaTracks from playlist {yaPlaylistId} and {normalizedTracks.Length} vkTracks");

            return new SyncTracks(normalizedTracks, yaTracks).GetNewTracks();
        }

        public async Task<ImportResult[]> ImportTracksAsync(NormalizedTrack[] trackToImport, string playlistId,
            Guid userId,
            Action<(int Processed, int Total, ImportResult importResult, bool submitted)> percentsComplete = null)
        {
            var yandexApi = yandexService.GetClientAsync(userId);
            var existentTracks = await yandexApi.GetTracksAsync(playlistId).ConfigureAwait(false);

            var existentTracksHash = existentTracks.Select(x => x.Id);
            var foundTracks = new HashSet<string>(existentTracksHash);

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

            var tracksToChange = new List<TrackToChange>(newTracks.Length);
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

                    var trackHash = yaTrack.Id;
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

                    tracksToChange.Add(new TrackToChange {Id = trackHash });

                    importResult.ImportStatus = ImportStatus.Ok;
                }
                catch (SpotifyApiException ex)
                {
                    log.Info($"ERROR YandexApiError {ex.Message}");
                    importResult.ImportStatus = ImportStatus.Error;
                    importResult.Message = $"Error: {ex.Message}. StackTrace: {ex.StackTrace}";
                }
                finally
                {
                    result.Add(importResult);
                    percentsComplete?.Invoke((++completeCount, totalCount, importResult, false));
                    log.Info($"PROCESSED {completeCount}/{totalCount} tracks. NotFound {notFoundCount}");
                }
            }

            try
            {
                var playlists = await yandexApi.GetPlaylistsAsync().ConfigureAwait(false);
                var revision = playlists.FirstOrDefault(x => x.Id == playlistId)?.Revision;
                
                await yandexApi.AddTracksToPlaylistAsync(playlistId, revision, tracksToChange.ToArray()).ConfigureAwait(false);
                
                percentsComplete?.Invoke((completeCount, totalCount, null, true));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result.ToArray();
        }

        public async Task ImportTracksAsync(TrackToChange[] trackToImport, string playlistId, Guid userId)
        {
            var yandexApi = yandexService.GetClientAsync(userId);

            var existentTracks = await yandexApi.GetTracksAsync(playlistId).ConfigureAwait(false);

            var existentTrackHashes = new HashSet<string>(existentTracks.Select(track => track.Id));

            var newTracks = trackToImport.Where(x => !existentTrackHashes.Contains(x.Id)).ToArray();

            if (newTracks.Length == 0)
            {
                return;
            }

            var playlists = await yandexApi.GetPlaylistsAsync().ConfigureAwait(false);
            var revision = playlists.FirstOrDefault(x => x.Id == playlistId)?.Revision;
            await yandexApi.AddTracksToPlaylistAsync(playlistId, revision, newTracks).ConfigureAwait(false);
        }

        public async Task<YandexTrack[]> GetExistentTracksAsync(Guid userId, string playlistId)
        {
            var yandexApi = yandexService.GetClientAsync(userId);
            var existentTracks = await yandexApi.GetTracksAsync(playlistId).ConfigureAwait(false);
            return existentTracks.SelectMany(track => track.Albums.Select(album => new YandexTrack
            {
                Id = track.Id,
                Title = track.Title,
                AlbumId = album.Id,
                Artists = track.Artists.Select(x => x.Name).ToArray()
            })).ToArray();
        }
    }
}