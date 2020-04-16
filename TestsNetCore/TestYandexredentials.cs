using System.Threading.Tasks;
using Tolltech.YandexClient.Authorizations;

namespace Tolltech.TestsNetCore
{
    public class TestYandexredentials : IYandexCredentials
    {
        public Task<AuthorizationInfo> GetAuthorizationInfoAsync()
        {
            return Task.FromResult(new AuthorizationInfo
            {
                Token = "AQAAAAAvywioAAG8XkTNfuGQokvth97cbk83zY8",
                //my app
                //Token = "AQAAAAAvywioAAVrRE-tXzwPg0omkbxIka4R_KQ",
                //&token_type=bearer&expires_in=15549500
                Uid = "801835176"
            });
        }

        public string GetAuthorizeUrl()
        {
            throw new System.NotImplementedException();
        }
    }
}