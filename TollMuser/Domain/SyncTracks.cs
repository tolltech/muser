using System.Linq;
using Tolltech.Muser.Models;
using Tolltech.SpotifyClient.ApiModels;
using Vostok.Logging.Abstractions;

namespace Tolltech.Muser.Domain
{
    public class SyncTracks
    {
        private readonly NormalizedTrack[] normalizedTracks;

        private readonly YandexTracks yandexTracks;
        private static readonly ILog log = LogProvider.Get();

        public SyncTracks(NormalizedTrack[] normalizedTracks, Track[] yaTracks)
        {
            this.normalizedTracks = normalizedTracks;
            yandexTracks = new YandexTracks(yaTracks);
        }

        public NormalizedTrack[] GetNewTracks()
        {
            var newTracks = normalizedTracks
                .Where(x => yandexTracks.FindEqualTrack(x) == null)
                .ToArray();

            log.Info($"Found {newTracks.Length} new tracks");

            return newTracks;
        }
    }
}