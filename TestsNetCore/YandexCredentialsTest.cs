using System;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using Tolltech.Serialization;
using Tolltech.YandexClient.Authorizations;

namespace Tolltech.TestsNetCore
{
    public class YandexCredentialsTest : TestBase
    {
        private IYandexCredentials yandexCredentials;

        protected override void SetUp()
        {
            base.SetUp();

            container.Rebind<IYandexCredentials>().ToConstant(new YandexCredentials(container.Get<IJsonSerializer>(), "alexandrovpe2@yandex.ru", "tc_123456"));

            yandexCredentials = container.Get<IYandexCredentials>();
        }

        [Test]
        public async Task TestGetCredentials()
        {
            var actual = await yandexCredentials.GetAuthorizationInfoAsync();
            Assert.IsNotEmpty(actual.Token);
            Assert.IsNotEmpty(actual.Uid);
            Assert.True(actual.ExpirationDate > DateTime.Now);
        }
    }
}