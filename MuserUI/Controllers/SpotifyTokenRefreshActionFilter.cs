using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Tolltech.Muser.Settings;
using Tolltech.MuserUI.Common;
using Tolltech.MuserUI.Spotify;
using Tolltech.SpotifyClient;

namespace Tolltech.MuserUI.Controllers
{
    public class SpotifyTokenRefreshActionFilter  : Attribute, IActionFilter
    {
        private readonly ISpotifyTokenService spotifyTokenService;
        private readonly ISpotifyTokenClient spotifyTokenClient;
        private readonly IAuthorizationSettings authorizationSettings;

        public SpotifyTokenRefreshActionFilter(ISpotifyTokenService spotifyTokenService, ISpotifyTokenClient spotifyTokenClient, IAuthorizationSettings authorizationSettings, ILogger<SpotifyTokenRefreshActionFilter> logger)
        {
            this.spotifyTokenService = spotifyTokenService;
            this.spotifyTokenClient = spotifyTokenClient;
            this.authorizationSettings = authorizationSettings;
        }
        
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.User.FindFirst(x => x.Type == Constants.UserIdClaim)?.Value.SafeToGuid();
            if (userId == null) return;
            
            var existentToken = spotifyTokenService.Find(userId.Value).GetAwaiter().GetResult();
            if (existentToken == null) return;

            var utcNow = DateTime.UtcNow;
            if (existentToken.ExpiresUtc > utcNow)
            {
                authorizationSettings.SetMuserAuthorization(new MuserAuthorization
                            {
                                SpotifyAccessToken = existentToken.AccessToken,
                                SpotifyAccessTokenExpiresDate = existentToken.ExpiresUtc
                            }, userId.Value);
                return;
            }

            
            var newToken = spotifyTokenClient.RefreshToken(existentToken.RefreshToken).GetAwaiter().GetResult();
            existentToken.AccessToken = newToken.AccessToken;
            existentToken.ExpiresUtc = DateTime.UtcNow.AddSeconds(newToken.ExpiresIn).AddMinutes(-5);
            
            spotifyTokenService.CreateOrUpdate(existentToken, userId.Value).GetAwaiter().GetResult();
            
            authorizationSettings.SetMuserAuthorization(new MuserAuthorization
            {
                SpotifyAccessToken = newToken.AccessToken,
                SpotifyAccessTokenExpiresDate = existentToken.ExpiresUtc
            }, userId.Value);
        }
    }
}