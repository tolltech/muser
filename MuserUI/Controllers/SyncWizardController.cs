﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tolltech.Muser.Domain;
using Tolltech.Muser.Models;
using Tolltech.Muser.Settings;
using Tolltech.MuserUI.Common;
using Tolltech.MuserUI.Extensions;
using Tolltech.MuserUI.Models.Sync;
using Tolltech.MuserUI.Models.SyncWizard;
using Tolltech.MuserUI.Sync;
using Tolltech.Serialization;
using Tolltech.SqlEF;
using Tolltech.YandexClient.ApiModels;

namespace Tolltech.MuserUI.Controllers
{
    [Authorize]
    [Route("sync")]
    public class SyncWizardController : BaseController
    {
        private readonly ITempSessionService tempSessionService;
        private readonly ITrackGetter trackGetter;
        private readonly IQueryExecutorFactory queryExecutorFactory;
        private readonly IYandexService yandexService;
        private readonly IAuthorizationSettings authorizationSettings;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IJsonTrackGetter jsonTrackGetter;
        private readonly IDomainService domainService;
        private readonly IProgressBar progressBar;
        private readonly IImportResultLogger importResultLogger;
        private readonly ICaptcha captcha;

        public SyncWizardController(ITempSessionService tempSessionService, ITrackGetter trackGetter,
            IQueryExecutorFactory queryExecutorFactory,
            IYandexService yandexService, IAuthorizationSettings authorizationSettings,
            IJsonSerializer jsonSerializer,
            IJsonTrackGetter jsonTrackGetter,
            IDomainService domainService,
            IProgressBar progressBar,
            IImportResultLogger importResultLogger,
            ICaptcha captcha)
        {
            this.tempSessionService = tempSessionService;
            this.trackGetter = trackGetter;
            this.queryExecutorFactory = queryExecutorFactory;
            this.yandexService = yandexService;
            this.authorizationSettings = authorizationSettings;
            this.jsonSerializer = jsonSerializer;
            this.jsonTrackGetter = jsonTrackGetter;
            this.domainService = domainService;
            this.progressBar = progressBar;
            this.importResultLogger = importResultLogger;
            this.captcha = captcha;
        }

        [HttpGet("")]
        public async Task<ActionResult> Index(Guid? sessionId)
        {
            if (!sessionId.HasValue)
            {
                return View();
            }

            var tracksText =
                await tempSessionService.FindSessionTextAsync(sessionId.Value, UserId).ConfigureAwait(true);

            if (tracksText?.IsNullOrWhitespace() ?? true)
            {
                return View();
            }

            var sourceTracks = trackGetter.GetTracks(tracksText);
            return View("InputTracks", sourceTracks.ToTracksWizardModel());
        }

        [HttpPost("inputtracksexternal")]
        [AllowAnonymous]
        [EnableCors(Constants.MuserCorsPolicy)]
        public async Task<JsonResult> GetInputTracksExternal([FromBody] ExternalInputTracksModel inputTracks)
        {
            var sessionId = Guid.NewGuid();

            await tempSessionService.SaveTempSessionAsync(sessionId, SafeUserId, inputTracks?.Text ?? string.Empty)
                .ConfigureAwait(true);

            var url = Url.Action("Index", new {sessionId = sessionId});
            return Json(new {Url = url});
        }

        [HttpPost("inputtracks")]
        public ActionResult GetInputTracks(InputTracksModel inputTracks)
        {
            var sourceTracks = trackGetter.GetTracks(inputTracks.Text);
            return View("InputTracks", sourceTracks.ToTracksWizardModel());
        }

        [HttpPost("inputtracksfromfile")]
        public async Task<ActionResult> GetInputTracksFromFile(IFormFile uploadedFile)
        {
            await using var memoryStream = new MemoryStream();
            await uploadedFile.CopyToAsync(memoryStream).ConfigureAwait(true);

            var fileBody = Encoding.UTF8.GetString(memoryStream.ToArray());

            var playlist = new PlaylistDbo
            {
                Id = Guid.NewGuid(),
                Content = fileBody,
                Date = DateTime.Now,
                UserId = UserId,
                Filename = uploadedFile.FileName,
                Extension = new string(uploadedFile.FileName?.Reverse().TakeWhile(c => c != '.').Reverse().ToArray() ??
                                       Array.Empty<char>())
            };
            using var queryExecutor = queryExecutorFactory.Create<PlaylistsHandler, PlaylistDbo>();
            await queryExecutor.ExecuteAsync(f => f.CreateAsync(playlist)).ConfigureAwait(true);

            var sourceTracks = trackGetter.GetTracks(fileBody);
            return View("InputTracks", sourceTracks.ToTracksWizardModel());
        }

        [HttpPost("savetracks")]
        public async Task<ActionResult> SaveTracks(InputTracksSessionForm tracksSessionForm)
        {
            var tracksModel = jsonSerializer.DeserializeFromString<TracksWizardModel>(tracksSessionForm.TracksJson);
            var sourceTracks = tracksModel.ToTracksWizardModel();
            var text = jsonTrackGetter.SerializeTracks(sourceTracks);

            var sessionId = Guid.NewGuid();
            await tempSessionService.SaveTempSessionAsync(sessionId, SafeUserId, text).ConfigureAwait(true);

            if (authorizationSettings.UserAuthorized(UserId))
            {
                return RedirectToAction("GetYaPlaylists", new {sessionId = sessionId});
            }

            return RedirectToAction("YandexAuthorize", new {sessionId = sessionId});
        }

        [HttpGet("yaauthorize")]
        public ActionResult YandexAuthorize(Guid sessionId)
        {
            var yandexCookie = Request.FindCookies(Constants.YaLoginCookie);
            return View(new YandexAuthorizeForm
            {
                Login = yandexCookie
            });
        }

        [HttpPost("yaauthorize")]
        public async Task<ActionResult> YandexAuthorize(YandexAuthorizeForm authorizeForm, Guid sessionId)
        {
            if (captcha.IsForbid(UserId, "Yandex"))
            {
                return View("YandexAuthorize", new YandexAuthorizeForm {Login = authorizeForm.Login, Forbid = true});
            }

            var success = await yandexService.CheckCredentialsAsync(authorizeForm.Login, authorizeForm.Pass)
                .ConfigureAwait(true);

            Response.SetCookies(Constants.YaLoginCookie, authorizeForm.Login ?? string.Empty);

            if (!success)
            {
                captcha.IncrementForbid(UserId, "Yandex");
                return View("YandexAuthorize",
                    new YandexAuthorizeForm {Login = authorizeForm.Login, AuthFailed = true});
            }

            var authInfo = authorizationSettings.GetCachedMuserAuthorization(UserId);
            if (authInfo == null)
            {
                authInfo = new MuserAuthorization
                {
                    YaLogin = authorizeForm.Login,
                    YaPassword = authorizeForm.Pass
                };
            }
            else
            {
                authInfo.YaLogin = authorizeForm.Login;
                authInfo.YaPassword = authorizeForm.Pass;
            }

            authorizationSettings.SetMuserAuthorization(authInfo, UserId);

            return await GetYaPlaylists(sessionId).ConfigureAwait(true);
        }

        [HttpGet("yaplaylists")]
        public async Task<ActionResult> GetYaPlaylists(Guid sessionId)
        {
            var client = await yandexService.GetClientAsync(UserId).ConfigureAwait(true);

            var playlists = await client.GetPlaylistsAsync().ConfigureAwait(true);

            return View("YaPlaylists", new YaPlaylistsForm
            {
                Playlists = playlists
                    .Select(x => new YaPlaylist
                    {
                        Title = x.Title,
                        Id = x.Id
                    })
                    .ToArray(),
                SelectedPlaylistId = playlists.FirstOrDefault(x => x.Title.ToLower() == "vk")?.Id,
                Login = client.Login
            });
        }

        [HttpPost("import")]
        public async Task<ActionResult> ImportTracks(YaPlaylistsForm yaPlaylists, Guid sessionId)
        {
            var yaPlayListId = yaPlaylists.SelectedPlaylistId;

            if (yaPlayListId.IsNullOrWhitespace())
            {
                return RedirectToAction("GetYaPlaylists", new {sessionId = sessionId});
            }

            var tracksText = await tempSessionService.FindSessionTextAsync(sessionId, UserId).ConfigureAwait(true);
            var sourceTracks = trackGetter.GetTracks(tracksText);

            var trackToImport = sourceTracks
                .Select(x => new NormalizedTrack
                {
                    Artist = x.Artist,
                    Title = x.Title,
                })
                .Reverse()
                .ToArray();

            var progressId = Guid.NewGuid();

#pragma warning disable 4014
            RunImport(progressId, trackToImport, yaPlayListId, sessionId, UserId);
#pragma warning restore 4014

            var cnt = 0;
            while (progressBar.FindProgressModel(progressId) == null && cnt < 300)
            {
                await Task.Delay(100).ConfigureAwait(true);
                ++cnt;
            }

            return View("ImportProgress", new ProgressWithUrlModel
            {
                Progress = progressBar.FindProgressModel(progressId) ??
                           new ProgressModel {Id = progressId, Processed = 0, Total = 0, SessionId = sessionId},
                YandexPlaylistUrl = GetPlaylistUrl(yaPlaylists.Login, yaPlayListId),
                SessionId = sessionId
            });
        }

        private static string GetPlaylistUrl([CanBeNull] string yandexEmail, string yaPlaylistId)
        {
            var loginChars = yandexEmail?.TakeWhile(c => c != '@').ToArray();
            var yaPlaylistsLogin = new string(loginChars ?? Array.Empty<char>());
            return $"https://music.yandex.ru/users/{yaPlaylistsLogin}/playlists/{yaPlaylistId}";
        }

        private async Task RunImport(Guid progressId, NormalizedTrack[] trackToImport, string yaPlayListId,
            Guid sessionId, Guid userId)
        {
            void UpdateProgress((int Processed, int Total, ImportResult importResult) tuple)
            {
                var currentProgress = progressBar.FindProgressModel(progressId) ?? new ProgressModel
                {
                    Id = progressId,
                    Total = tuple.Total,
                    Processed = tuple.Processed,
                    SessionId = sessionId
                };

                currentProgress.Total = tuple.Total;
                currentProgress.Processed = tuple.Processed;

                var importResult = tuple.importResult;
                if (importResult.ImportStatus == ImportStatus.Error ||
                    importResult.ImportStatus == ImportStatus.NotFound)
                {
                    currentProgress.Errors.Add((new TrackModel
                    {
                        Title = importResult.ImportingTrack?.Title ?? string.Empty,
                        Artist = importResult.ImportingTrack?.Artist ?? string.Empty
                    }, $"{jsonSerializer.SerializeToString(new {importResult.ImportStatus, importResult.Message})}"));
                }

                progressBar.UpdateProgressModel(currentProgress);
            }

            var results = await domainService.ImportTracksAsync(trackToImport, yaPlayListId, userId, UpdateProgress)
                .ConfigureAwait(false);

            await importResultLogger.WriteImportLogsAsync(results, userId, sessionId).ConfigureAwait(false);

            var lastProgress = progressBar.FindProgressModel(progressId);
            lastProgress.ImportLogsSaved = true;
            progressBar.UpdateProgressModel(lastProgress);
        }

        [HttpGet("progress")]
        public async Task<ActionResult> GetImportProgress(Guid progressId)
        {
            var progressModel = progressBar.FindProgressModel(progressId);

            var logsSaved = false;

            if (progressModel.SessionId.HasValue && progressModel.Total == progressModel.Processed)
            {
                var count = await importResultLogger.CountAsync(progressModel.SessionId.Value, UserId).ConfigureAwait(true);
                if (count >= progressModel.Total)
                {
                    logsSaved = true;
                }
            }

            return PartialView("ImportProgressPartial", new ProgressModel
            {
                Total = progressModel.Total,
                Processed = progressModel.Processed,
                Errors = progressModel.Errors.ToList(),
                Id = progressModel.Id,
                ImportLogsSaved = logsSaved,
                SessionId = progressModel.SessionId
            });
        }

        [HttpGet("reimport")]
        public async Task<ActionResult> ReImport(Guid sessionId)
        {
            if (!authorizationSettings.UserAuthorized(UserId))
            {
                return RedirectToAction("YandexAuthorize", new {sessionId = sessionId});
            }

            var allImportResults = await importResultLogger.SelectAsync(sessionId, UserId).ConfigureAwait(true);

            var errorImportResults = allImportResults.Where(x => x.Status == ImportStatus.Error).ToArray();

            var notFoundImportResults = allImportResults.Where(x => x.Status == ImportStatus.NotFound).ToArray();
            var playlistId = notFoundImportResults.Select(x => x.PlaylistId)
                .FirstOrDefault(x => !x.IsNullOrWhitespace());

            var existentTracks = await domainService.GetExistentTracksAsync(UserId, playlistId).ConfigureAwait(true);
            var existentTrackHashes = new HashSet<(string, string)>(existentTracks.Select(x => (x.Id, x.AlbumId)));

            var errors = notFoundImportResults
                .Where(x => !existentTrackHashes.Contains((x.CandidateTrackId, x.CandidateAlbumId)))
                .Select(x => new ReImportTrack
                {
                    Artist = x.CandidateArtist,
                    Title = x.CandidateTitle,
                    TrackId = x.CandidateTrackId,
                    AlbumId = x.CandidateAlbumId,
                    Selected = false,
                    InputArtist = x.Artist,
                    InputTitle = x.Title,
                    Id = x.Id,
                    Disabled = x.CandidateAlbumId.IsNullOrWhitespace() || x.CandidateAlbumId.IsNullOrWhitespace()
                })
                .ToArray();

            return View(new ReImportModel
            {
                Tracks = errors,
                SessionId = sessionId,
                PlaylistId = playlistId,
                PlaylistUrl = GetPlaylistUrl(authorizationSettings.GetCachedMuserAuthorization(UserId)?.YaLogin,
                    playlistId),
                Total = allImportResults.Length,
                Success = allImportResults.Length - notFoundImportResults.Length - errorImportResults.Length
            });
        }

        [HttpPost("reimport")]
        public async Task<ActionResult> ReImport(ReImportFormJson reImportModelJson)
        {
            var reImportModel = jsonSerializer.DeserializeFromString<ReImportForm>(reImportModelJson.ReImportForm);

            if (!authorizationSettings.UserAuthorized(UserId))
            {
                return RedirectToAction("YandexAuthorize", new {sessionId = reImportModel.SessionId});
            }

            var selected = reImportModel?.Tracks?.ToArray() ?? Array.Empty<ReImportTrackForm>();
            var trackToChange = selected
                .Select(x => new TrackToChange
                {
                    AlbumId = x.AlbumId,
                    Id = x.TrackId
                })
                .ToArray();
            await domainService.ImportTracksAsync(trackToChange, reImportModel.PlaylistId, UserId).ConfigureAwait(true);

            await importResultLogger.UpdateManualApprovingAsync(selected.Select(x => x.Id).ToArray())
                .ConfigureAwait(true);

            var url = GetPlaylistUrl(authorizationSettings.GetCachedMuserAuthorization(UserId)?.YaLogin,
                reImportModel.PlaylistId);
            return RedirectToAction("ReImport", new {sessionId = reImportModel.SessionId});
        }
    }
}