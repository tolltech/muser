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
using Tolltech.Muser.Settings;
using Tolltech.MuserUI.Common;
using Tolltech.MuserUI.Extensions;
using Tolltech.MuserUI.Models.Sync;
using Tolltech.MuserUI.Models.SyncWizard;
using Tolltech.MuserUI.Sync;
using Tolltech.Serialization;
using Tolltech.SqlEF;

namespace Tolltech.MuserUI.Controllers
{
    [Authorize]
    [Route("syncwizzard")]
    public class SyncWizardController : BaseController
    {
        private readonly ITempSessionService tempSessionService;
        private readonly ITrackGetter trackGetter;
        private readonly IQueryExecutorFactory queryExecutorFactory;
        private readonly IYandexService yandexService;
        private readonly IAuthorizationSettings authorizationSettings;
        private readonly IJsonSerializer jsonSerializer;
        private readonly IJsonTrackGetter jsonTrackGetter;

        public SyncWizardController(ITempSessionService tempSessionService, ITrackGetter trackGetter,
            IQueryExecutorFactory queryExecutorFactory,
            IYandexService yandexService, IAuthorizationSettings authorizationSettings,
            IJsonSerializer jsonSerializer,
            IJsonTrackGetter jsonTrackGetter)
        {
            this.tempSessionService = tempSessionService;
            this.trackGetter = trackGetter;
            this.queryExecutorFactory = queryExecutorFactory;
            this.yandexService = yandexService;
            this.authorizationSettings = authorizationSettings;
            this.jsonSerializer = jsonSerializer;
            this.jsonTrackGetter = jsonTrackGetter;
        }

        [HttpGet("")]
        public async Task<ActionResult> Index(Guid? sessionId)
        {
            if (!sessionId.HasValue)
            {
                return View();
            }

            var tracksText = await tempSessionService.FindSessionTextAsync(sessionId.Value, UserId).ConfigureAwait(true);

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
            var success = await yandexService.CheckCredentialsAsync(authorizeForm.Login, authorizeForm.Pass)
                .ConfigureAwait(true);

            Response.SetCookies(Constants.YaLoginCookie, authorizeForm.Login ?? string.Empty);

            if (!success)
            {
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
    }
}