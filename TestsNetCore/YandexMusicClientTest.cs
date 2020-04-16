using System;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using Tolltech.Serialization;
using Tolltech.YandexClient;
using Tolltech.YandexClient.ApiModels;
using Tolltech.YandexClient.Authorizations;

namespace Tolltech.TestsNetCore
{
    public class YandexMusicClientTest : TestBase
    {
        private IYandexMusicClient yandexMusicClient;

        protected override void SetUp()
        {
            base.SetUp();

            container.Rebind<IYandexCredentials>().ToConstant(new YandexCredentials(container.Get<IJsonSerializer>(), "alexandrovpe2@yandex.ru", "tc_123456"));
            yandexMusicClient = container.Get<IYandexMusicClient>();
        }

        [Test]
        public async Task TestGetPlaylists()
        {
            var actuals = await yandexMusicClient.GetPlaylistsAsync().ConfigureAwait(false);
            Assert.IsNotEmpty(actuals);
            Assert.IsNotEmpty(actuals.First().Revision);
            Assert.IsNotEmpty(actuals.First().Id);
            Assert.IsNotEmpty(actuals.First().Title);
        }

        [Test]
        public async Task TestCreateAndDeletePlaylist()
        {
            var title = Guid.NewGuid().ToString();
            var actual = await yandexMusicClient.CreatePlaylistAsync(title).ConfigureAwait(false);
            Assert.IsNotEmpty(actual.Id);
            Assert.AreEqual(title, actual.Title);

            var playlists = await yandexMusicClient.GetPlaylistsAsync().ConfigureAwait(false);
            Assert.IsTrue(playlists.Any(x => x.Id == actual.Id && x.Title == actual.Title));

            await yandexMusicClient.DeletePlaylistAsync(actual.Id).ConfigureAwait(false);

            playlists = await yandexMusicClient.GetPlaylistsAsync().ConfigureAwait(false);
            Assert.IsFalse(playlists.Any(x => x.Id == actual.Id && x.Title == actual.Title));
        }

        [Test]
        public async Task TestGetPlaylistTracks()
        {
            var actuals = await yandexMusicClient.GetTracksAsync("1008").ConfigureAwait(false);
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
            var playlists = await yandexMusicClient.GetPlaylistsAsync().ConfigureAwait(false);
            var playlist = playlists.FirstOrDefault(x => x.Id == "1008" || x.Title == "Test2");

            Assert.IsNotNull(playlist);

            var playlistId = playlist.Id;
            var revision = playlist.Revision;

            var actuals = await yandexMusicClient.GetTracksAsync(playlistId).ConfigureAwait(false);
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

            await yandexMusicClient.RemoveTracksToPlaylistAsync(playlistId, revision, new[] {trackToChange})
                .ConfigureAwait(false);

            playlists = await yandexMusicClient.GetPlaylistsAsync().ConfigureAwait(false);
            playlist = playlists.FirstOrDefault(x => x.Id == playlistId);

            Assert.IsNotNull(playlist);
            Assert.AreNotEqual(playlist.Revision, revision);

            revision = playlist.Revision;

            actuals = await yandexMusicClient.GetTracksAsync(playlistId).ConfigureAwait(false);
            Assert.IsFalse(actuals.Any(x => x.Id == trackId && x.Albums.Any(y => y.Id == albumId)));

            await yandexMusicClient.AddTracksToPlaylistAsync(playlistId, revision, new[] { trackToChange })
                .ConfigureAwait(false);

            actuals = await yandexMusicClient.GetTracksAsync(playlistId).ConfigureAwait(false);
            Assert.IsTrue(actuals.Any(x => x.Id == trackId && x.Albums.Any(y => y.Id == albumId)));
        }

        [Test]
        public async Task TestSearch()
        {
            var actuals = await yandexMusicClient.SearchAsync("Король и шут камнем по голове").ConfigureAwait(false);
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
            Assert.Throws<YandexApiException>(() => yandexMusicClient.SearchAsync("The Rasmus - Living In A World Without You #").GetAwaiter().GetResult());
        }
    }
}