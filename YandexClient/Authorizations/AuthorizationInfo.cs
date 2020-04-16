using System;

namespace Tolltech.YandexClient.Authorizations
{
    public class AuthorizationInfo
    {
        public string Token { get; set; }
        public string Uid { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}