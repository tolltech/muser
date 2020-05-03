using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Tolltech.MuserUI.Sync;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Controllers
{
    [Authorize]
    [Route("sync")]
    public class SyncController : BaseController
    {
        private readonly IImportResultLogger importResultLogger;
        private readonly IQueryExecutorFactory queryExecutorFactory;
        private Guid UserId => SafeUserId.Value;

        private readonly IYandexService yandexService;
        private readonly IAuthorizationSettings authorizationSettings;
        private readonly IDomainService domainService;
        private readonly ITrackGetter trackGetter;
        private readonly IProgressBar progressBar;

        public SyncController(IYandexService yandexService, IAuthorizationSettings authorizationSettings,
            IDomainService domainService, ITrackGetter trackGetter, IProgressBar progressBar,
            IImportResultLogger importResultLogger, IQueryExecutorFactory queryExecutorFactory
        )
        {
            this.yandexService = yandexService;
            this.authorizationSettings = authorizationSettings;
            this.domainService = domainService;
            this.trackGetter = trackGetter;
            this.progressBar = progressBar;
            this.importResultLogger = importResultLogger;
            this.queryExecutorFactory = queryExecutorFactory;
        }

        [HttpGet("")]
        public ActionResult Index(Guid? sessionId)
        {
            if (!sessionId.HasValue)
            {
                return View();
            }

            return View();
        }

        [HttpGet("yatracks")]
        public async Task<ActionResult> GetYaTracks(string yaPlaylistId)
        {
            var client = await yandexService.GetClientAsync(UserId).ConfigureAwait(true);

            var tracks = await client.GetTracksAsync(yaPlaylistId).ConfigureAwait(true);

            return PartialView("Tracks", tracks.ToTracksModel());
        }

        [HttpPost("unauthorizeya")]
        public void UnauthorizeYa()
        {
            var authInfo = authorizationSettings.GetCachedMuserAuthorization(UserId);
            if (authInfo == null)
            {
                return;
            }
            else
            {
                authInfo.YaLogin = string.Empty;
                authInfo.YaPassword = string.Empty;
            }

            authorizationSettings.SetMuserAuthorization(authInfo, UserId);
        }

        [HttpPost("authorizeya")]
        public async Task<ActionResult> AuthorizeYa(AuthorizeForm authorizeForm)
        {
            var success = await yandexService.CheckCredentialsAsync(authorizeForm.Login, authorizeForm.Pass)
                .ConfigureAwait(true);

            Response.SetCookies(Constants.YaLoginCookie, authorizeForm.Login ?? string.Empty);

            if (!success)
            {
                return PartialView("AuthorizeResult", new AuthorizeResult {Success = false});
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

            return await GetYaPlaylists().ConfigureAwait(true);
        }

        [HttpGet("yaplaylists")]
        public async Task<ActionResult> GetYaPlaylists()
        {
            var client = await yandexService.GetClientAsync(UserId).ConfigureAwait(true);

            var playlists = await client.GetPlaylistsAsync().ConfigureAwait(true);

            return PartialView("YaPlaylists", new YaPlaylists
            {
                Playlists = playlists
                    .Select(x => new YaPlaylist
                    {
                        Title = x.Title,
                        Id = x.Id
                    })
                    .ToArray()
            });
        }

        [HttpPost("inputtracksexternal")]
        [AllowAnonymous]
        [EnableCors(Constants.MuserCorsPolicy)]
        public async Task<string> GetInputTracksExternal([FromBody] InputTracksModel inputTracks)
        {
            var sessionId = Guid.NewGuid();
            var sessionDbo = new TempSessionDbo
            {
                UserId = SafeUserId,
                Text = inputTracks?.Text ?? string.Empty,
                Date = DateTime.UtcNow,
                Id = sessionId
            };

            using var queryExecutor = queryExecutorFactory.Create<TempSessionHandler, TempSessionDbo>();
            await queryExecutor.ExecuteAsync(x => x.CreateAsync(sessionDbo)).ConfigureAwait(true);
            return Url.Action("Index", new {sessionId = sessionId});
        }

        [HttpPost("inputtracks")]
        public ActionResult GetInputTracks(InputTracksModel inputTracks)
        {
            var sourceTracks = trackGetter.GetTracks(inputTracks.Text);
            return PartialView("Tracks", sourceTracks.ToTracksModel());
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
            return PartialView("Tracks", sourceTracks.ToTracksModel());
        }

        [HttpPost("newtracks")]
        public async Task<ActionResult> GetNewTracks()
        {
            var tracksForm = await GetFromBodyAsync<TrackImportForm>().ConfigureAwait(true);
            var inputTracks = tracksForm.Tracks.ToTracksModel();
            var tracks = await domainService.GetNewTracksAsync(tracksForm.YaPlaylistId, UserId, inputTracks)
                .ConfigureAwait(true);
            return PartialView("Tracks", tracks.ToTracksModel());
        }

        [HttpPost("import")]
        public async Task<ActionResult> ImportTracks([FromBody] TrackImportForm tracksForm)
        {
            var trackToImport = tracksForm.Tracks.Tracks
                .Select(x => new NormalizedTrack
                {
                    Artist = x.Artist,
                    Title = x.Title,
                })
                .Reverse()
                .ToArray();

            var progressId = Guid.NewGuid();

            var results = await domainService.ImportTracksAsync(trackToImport, tracksForm.YaPlaylistId, UserId, tuple =>
                progressBar.UpdateProgressModel(new ProgressModel
                {
                    Id = progressId,
                    Proecssed = tuple.Processed,
                    Total = tuple.Total
                })).ConfigureAwait(true);

            await importResultLogger.WriteImportLogsAsync(results, UserId).ConfigureAwait(true);

            return PartialView("Progress", progressBar.GetProgressModel(progressId));
        }
    }
}