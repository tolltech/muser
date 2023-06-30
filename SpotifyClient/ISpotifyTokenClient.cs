using System.Threading.Tasks;

namespace Tolltech.SpotifyClient
{
    public interface ISpotifyTokenClient
    {
        Task<TokenResponse> GetAppToken();
        Task<TokenResponse> ExchangeToken(string authCode);
        Task<TokenResponse> RefreshToken(string refreshToken);
    }
}