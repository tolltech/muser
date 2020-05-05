using System.Threading.Tasks;

namespace Tolltech.YandexClient.Authorizations
{
    public interface IYandexCredentials
    {
        string Login { get; }
        Task<AuthorizationInfo> GetAuthorizationInfoAsync();
        string GetAuthorizeUrl();
    }
}