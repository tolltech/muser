using System.Threading.Tasks;
using FluentAssertions;
using Ninject;
using NUnit.Framework;
using Tolltech.SpotifyClient;
using Tolltech.SpotifyClient.Integration;

namespace Tolltech.TestsNetCore
{
    public class SpotifyTokenClientTest : TestBase
    {
        class SpotifyClientConfiguration : ISpotifyClientConfiguration
        {
            public string ClientId => "WriteHere";
            public string ClientSecret => "WriteHere";
        }
        
        private ISpotifyTokenClient spotifyTokenClient;

        protected override void SetUp()
        {
            base.SetUp();

            container.Rebind<ISpotifyClientConfiguration>().To<SpotifyClientConfiguration>();
            spotifyTokenClient = container.Get<ISpotifyTokenClient>();
        }

        [Test]
        //[Ignore("NeedClientSecret")]
        public async Task TestGetAppToken()
        {
            var token = await spotifyTokenClient.GetAppToken().ConfigureAwait(false);
            token.Should().NotBeNull();
            token.AccessToken.Should().NotBeNull();
            token.ExpiresIn.Should().BeGreaterThan(0);
            token.TokenType.Should().Be("Bearer");
        }
    }
}