using System.Linq;
using log4net;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public class SyncTracks
    {
        private readonly NormalizedTrack[] normalizedTracks;

        private readonly YandexTracks yandexTracks;
        private static readonly ILog log = LogManager.GetLogger(typeof(SyncTracks));

        public SyncTracks(NormalizedTrack[] normalizedTracks, YandexClient.ApiModels.Track[] yaTracks)
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