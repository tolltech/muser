using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tolltech.Muser.Settings;
using Tolltech.MuserUI.Common;
using Tolltech.MuserUI.Spotify;
using Tolltech.SpotifyClient;

namespace Tolltech.MuserUI.Controllers
{
    [Authorize]
    [Route("spotify")]
    public class SpotifyController : BaseController
    {
        private readonly ISpotifyTokenClient spotifyTokenClient;
        private readonly ISpotifyTokenService spotifyTokenService;
        private readonly IAuthorizationSettings authorizationSettings;

        public SpotifyController(ISpotifyTokenClient spotifyTokenClient, ISpotifyTokenService spotifyTokenService, IAuthorizationSettings authorizationSettings)
        {
            this.spotifyTokenClient = spotifyTokenClient;
            this.spotifyTokenService = spotifyTokenService;
            this.authorizationSettings = authorizationSettings;
        }
        
        [HttpGet("callback")]
        public async Task<ActionResult> Callback(string code, string state, string error)
        {
            //todo: тут со стейтом дыра конечно
            if (!error.IsNullOrWhitespace() || code.IsNullOrWhitespace())
            {
                return RedirectToAction("Index", "Home");
            }

            var token = await spotifyTokenClient.ExchangeToken(code).ConfigureAwait(true);

            var newToken = new TokenInfo
            {
                Scope = token.Scope,
                RefreshToken = token.RefreshToken,
                AccessToken = token.AccessToken,
                TokenType = token.TokenType,
                ExpiresUtc = DateTime.UtcNow.AddSeconds(token.ExpiresIn).AddMinutes(-5)
            };
            await spotifyTokenService.CreateOrUpdate(newToken, UserId).ConfigureAwait(true);

            authorizationSettings.SetMuserAuthorization(new MuserAuthorization
            {
                SpotifyAccessToken = newToken.AccessToken,
                SpotifyAccessTokenExpiresDate = newToken.ExpiresUtc
            }, UserId);

            return RedirectToAction("SpotifyAuthorized", "SyncWizard", new { sessionId = Guid.Parse(state) });
        }
    }
}