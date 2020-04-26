using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tolltech.Muser.Domain;
using Tolltech.Muser.Models;
using Tolltech.Muser.Settings;
using Tolltech.MuserUI.Common;
using Tolltech.MuserUI.Extensions;
using Tolltech.MuserUI.Models.Sync;
using Tolltech.MuserUI.Sync;

namespace Tolltech.MuserUI.Controllers
{
    [Route("sync")]
    public class SyncController : BaseController
    {
        private readonly IImportResultLogger importResultLogger;
        private Guid UserId => SafeUserId.Value;

        private readonly IYandexService yandexService;
        private readonly IAuthorizationSettings authorizationSettings;
        private readonly IDomainService domainService;
        private readonly ITrackGetter trackGetter;
        private readonly IProgressBar progressBar;

        public SyncController(IYandexService yandexService, IAuthorizationSettings authorizationSettings,
            IDomainService domainService, ITrackGetter trackGetter, IProgressBar progressBar, IImportResultLogger importResultLogger)
        {
            this.yandexService = yandexService;
            this.authorizationSettings = authorizationSettings;
            this.domainService = domainService;
            this.trackGetter = trackGetter;
            this.progressBar = progressBar;
            this.importResultLogger = importResultLogger;
        }

        [HttpGet("")]
        public ActionResult Index()
        {
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

        [HttpGet("newvktracks")]
        public async Task<ActionResult> GetNewVkTracks(string audioStr, string yaPlaylistId)
        {
            var audioUrl = trackGetter.GetAudioUrl(audioStr);
            var tracks = await domainService.GetNewVkTracksUnauthorizedAsync(yaPlaylistId, UserId, audioUrl)
                .ConfigureAwait(true);
            return PartialView("Tracks", tracks.ToTracksModel());
        }


        [HttpPost("import")]
        public async Task<ActionResult> ImportTracks(TrackImportForm tracksForm)
        {
            var trackToImport = tracksForm.Tracks.Tracks
                .Select(x => new VkTrack
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