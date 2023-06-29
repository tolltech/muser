using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MusicClientCore;
using Tolltech.Serialization;

namespace Tolltech.YandexClient.Authorizations
{
    public class YandexCredentials : IYandexCredentials
    {
        private readonly IJsonSerializer serializer;
        private readonly string login;
        private readonly string password;

        private readonly string clientId = "0618394846eb4d9589a602f80ce013d6";
        private readonly string clientSecret = "c13b3de8d9f5492caf321467c3520358";

        private readonly string tokenClientId = "23cabbbdc6cd418abb4b39c32c41195d";
        private readonly string tokenClientSecret = "53bc75238f0c4d08a118e51fe9203300";

        //my app
        //private readonly string clientId = "b57a03a55df04cc2b8ab2c1e909479ef";
        //private readonly string clientSecret = "cdf6958b3c5b42388c451221cfb71270";

        //private readonly string tokenClientId = null;
        //private readonly string tokenClientSecret = null;

        private readonly string deviceId = "377c5ae26b09fccd72deae0a95425559";
        private readonly string deviceUuid = "3cfccdaf75dcf98b917a54afe50447ba";
        private readonly string deviecePackageName = "ru.yandex.music";


        private static readonly string uriPrefix = "https://oauth.mobile.yandex.net/1/token";

        public YandexCredentials(IJsonSerializer serializer, string login, string password)
        {
            this.serializer = serializer;
            this.login = login;
            this.password = password;
        }

        private AuthorizationInfo authorizationInfo;

        public string Login => login;

        public async Task<AuthorizationInfo> GetAuthorizationInfoAsync()
        {
            if (authorizationInfo != null)
            {
                return authorizationInfo;
            }

            var firstBody = new
            {
                grant_type = "password",
                username = login,
                password = password,
                client_id = clientId,
                client_secret = clientSecret
            };

            var firstToken = await Post<FirstStepResponse>(uriPrefix, firstBody).ConfigureAwait(false);

            var parameters = new Dictionary<string, string>
            {
                {"device_id", deviceId},
                {"uuid", deviceUuid},
                {"package_name", deviecePackageName}
            };
            var queryString = $"{uriPrefix}?{parameters.ToUriParams()}";

            var secondBody = new
            {
                grant_type = "x-token",
                access_token = firstToken.AccessToken,
                client_id = tokenClientId ?? clientId,
                client_secret = tokenClientSecret ?? clientSecret
            };

            var secondToken = await Post<SecondStepResponse>(queryString, secondBody).ConfigureAwait(false);

            return authorizationInfo = new AuthorizationInfo
            {
                Token = secondToken.AccessToken,
                Uid = secondToken.Uid,
                ExpirationDate = DateTime.Now.AddMilliseconds(secondToken.Expires)
            };
        }

        public string GetAuthorizeUrl()
        {
            return $"https://oauth.yandex.ru/authorize?response_type=token&client_id={clientId}";
        }

        private async Task<T> Post<T>(string url, object body)
        {
            using (var webClient = new WebClient())
            {
                var firstResponse = await webClient.UploadDataTaskAsync(url, body.ToFormData()).ConfigureAwait(false);
                return serializer.Deserialize<T>(firstResponse);
            }
        }
    }
}