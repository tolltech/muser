using System.Threading.Tasks;

namespace Tolltech.YandexClient.Authorizations
{
    public interface IYandexCredentials
    {
        Task<AuthorizationInfo> GetAuthorizationInfoAsync();
        string GetAuthorizeUrl();
    }
}