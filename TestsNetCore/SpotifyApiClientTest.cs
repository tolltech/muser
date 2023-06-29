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
    public class SpotifyApiClientTest : TestBase
    {
        private ISpotifyApiClient spotifyApiClient;

        protected override void SetUp()
        {
            base.SetUp();

            spotifyApiClient = new SpotifyApiClient(@"WriteAccessTokenHere", container.Get<IJsonSerializer>());
        }

        [Test]
        [Ignore("NeedAccessToken")]
        public async Task TestGetAppToken()
        {
            var artistId = "0qc4X567Fs1DUbQ7bS2XSJ";
            var artist = await spotifyApiClient.GetArtist(artistId).ConfigureAwait(false);
            artist.Should().NotBeNull();
            artist.Id.Should().Be(artistId);
        }
    }
}