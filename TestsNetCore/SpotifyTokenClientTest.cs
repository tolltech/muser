using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Ninject;
using NUnit.Framework;
using Tolltech.Serialization;
using Tolltech.SpotifyClient;
using Tolltech.YandexClient;
using Tolltech.YandexClient.ApiModels;
using Tolltech.YandexClient.Authorizations;

namespace Tolltech.TestsNetCore
{
    public class SpotifyTokenClientTest : TestBase
    {
        private ISpotifyTokenClient spotifyTokenClient;

        protected override void SetUp()
        {
            base.SetUp();
            
            spotifyTokenClient = container.Get<ISpotifyTokenClient>();
        }

        [Test]
        //[Ignore("NeedClientSecret")]
        public async Task TestGetAppToken()
        {
            var token = await spotifyTokenClient.GetAppToken("9b7d3c73f0e94b128908b4c62ac9ead6",
                "WriteHereCLientSecret").ConfigureAwait(false);
            token.Should().NotBeNull();
            token.AccessToken.Should().NotBeNull();
            token.ExpiresIn.Should().BeGreaterThan(0);
            token.TokenType.Should().Be("Bearer");
        }
    }
}