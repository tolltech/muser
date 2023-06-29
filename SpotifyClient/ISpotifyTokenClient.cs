using System.Threading.Tasks;

namespace Tolltech.SpotifyClient
{
    public interface ISpotifyTokenClient
    {
        Task<TokenResponse> GetAppToken(string clientId, string clientSecret);
    }
}