using System.Linq;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public class SyncTracks
    {
        private readonly VkTrack[] vkTracks;

        private readonly YandexTracks yandexTracks;

        public SyncTracks(VkTrack[] vkTracks, YandexClient.ApiModels.Track[] yaTracks)
        {
            this.vkTracks = vkTracks;
            yandexTracks = new YandexTracks(yaTracks);
        }

        public VkTrack[] GetNewTracks()
        {
            var newTracks = vkTracks
                .Where(x => yandexTracks.FindEqualTrack(x) == null)
                .ToArray();

            //log.Info($"Found {newTracks.Length} new tracks");

            return newTracks;
        }
    }
}