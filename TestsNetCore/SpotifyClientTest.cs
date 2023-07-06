using System.Linq;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using Tolltech.Serialization;
using Tolltech.SpotifyClient;
using Tolltech.SpotifyClient.ApiModels;
using Tolltech.YandexClient;

namespace Tolltech.TestsNetCore
{
    public class SpotifyClientTest : TestBase
    {
        private ISpotifyApiClient spotifyApiClient;

        protected override void SetUp()
        {
            base.SetUp();

            var accessToken = @"";
            spotifyApiClient = new SpotifyApiClient(accessToken, container.Get<IJsonSerializer>());
        }

        [Test]
        public async Task TestGetPlaylists()
        {
            var actuals = await spotifyApiClient.GetPlaylistsAsync().ConfigureAwait(false);
            Assert.IsNotEmpty(actuals);
            Assert.IsNotEmpty(actuals.First().Revision);
            Assert.IsNotEmpty(actuals.First().Id);
            Assert.IsNotEmpty(actuals.First().Title);
        }

        [Test]
        public async Task TestGetPlaylistTracks()
        {
            var actuals = await spotifyApiClient.GetTracksAsync("75rQBJIASDsGBNrtqHxSvL").ConfigureAwait(false);
            Assert.IsNotEmpty(actuals);
            Assert.IsNotEmpty(actuals.First().Id);
            Assert.IsNotEmpty(actuals.First().Albums);
            Assert.IsNotEmpty(actuals.First().Artists);
            Assert.IsNotEmpty(actuals.First().Title);
            Assert.IsNotEmpty(actuals.First().Artists.First().Id);
            Assert.IsNotEmpty(actuals.First().Artists.First().Name);
            Assert.IsNotEmpty(actuals.First().Albums.First().Id);
            Assert.IsNotEmpty(actuals.First().Albums.First().Title);
        }

        [Test]
        public async Task TestChangePlaylistTracks()
        {
            var playlists = await spotifyApiClient.GetPlaylistsAsync().ConfigureAwait(false);
            var playlist = playlists.FirstOrDefault(x => x.Id == "75rQBJIASDsGBNrtqHxSvL" || x.Title == "Test2");

            Assert.IsNotNull(playlist);

            var playlistId = playlist.Id;
            var revision = playlist.Revision;

            var actuals = await spotifyApiClient.GetTracksAsync(playlistId).ConfigureAwait(false);
            Assert.IsNotEmpty(actuals);

            var trackId = actuals.First().Id;
            var albumId = actuals.First().Albums.First().Id;

            Assert.IsNotEmpty(trackId);
            Assert.IsNotEmpty(actuals.First().Albums);
            Assert.IsNotEmpty(actuals.First().Artists);
            Assert.IsNotEmpty(actuals.First().Title);
            Assert.IsNotEmpty(actuals.First().Artists.First().Id);
            Assert.IsNotEmpty(actuals.First().Artists.First().Name);
            Assert.IsNotEmpty(albumId);
            Assert.IsNotEmpty(actuals.First().Albums.First().Title);

            var trackToChange = new TrackToChange
            {
                Id = trackId,
                AlbumId = albumId
            };

            await spotifyApiClient.RemoveTracksToPlaylistAsync(playlistId, revision, new[] {trackToChange})
                .ConfigureAwait(false);

            playlists = await spotifyApiClient.GetPlaylistsAsync().ConfigureAwait(false);
            playlist = playlists.FirstOrDefault(x => x.Id == playlistId);

            Assert.IsNotNull(playlist);
            Assert.AreNotEqual(playlist.Revision, revision);

            revision = playlist.Revision;

            actuals = await spotifyApiClient.GetTracksAsync(playlistId).ConfigureAwait(false);
            Assert.IsFalse(actuals.Any(x => x.Id == trackId && x.Albums.Any(y => y.Id == albumId)));

            await spotifyApiClient.AddTracksToPlaylistAsync(playlistId, revision, new[] { trackToChange })
                .ConfigureAwait(false);

            actuals = await spotifyApiClient.GetTracksAsync(playlistId).ConfigureAwait(false);
            Assert.IsTrue(actuals.Any(x => x.Id == trackId && x.Albums.Any(y => y.Id == albumId)));
        }

        [Test]
        public async Task TestSearch()
        {
            var actuals = await spotifyApiClient.SearchAsync("Король и шут камнем по голове").ConfigureAwait(false);
            Assert.IsNotEmpty(actuals);
            Assert.IsNotEmpty(actuals.First().Id);
            Assert.IsNotEmpty(actuals.First().Albums);
            Assert.IsNotEmpty(actuals.First().Artists);
            Assert.IsNotEmpty(actuals.First().Title);
            Assert.IsNotEmpty(actuals.First().Artists.First().Id);
            Assert.IsNotEmpty(actuals.First().Artists.First().Name);
            Assert.IsNotEmpty(actuals.First().Albums.First().Id);
            Assert.IsNotEmpty(actuals.First().Albums.First().Title);
        }


        [Test]
        public void TestSearchWithBadSymbols()
        {
            Assert.Throws<SpotifyApiException>(() => spotifyApiClient.SearchAsync("The Rasmus - Living In A World Without You #").GetAwaiter().GetResult());
        }
    }
}