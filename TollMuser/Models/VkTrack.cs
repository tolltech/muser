using System;
using System.Linq;
using Tolltech.Muser.Domain;

namespace Tolltech.Muser.Models
{
    public class VkTrack
    {
        public string Artist { get; set; }
        public string Title { get; set; }

        public string[] Artists => Artist.Split(new[] {",", "&", " ft.", " ft ", " feat. " }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        public string[] NormalizedArtists
        {
            get
            {
                return Artists.Select(x => x.NormalizeTrackInfo())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();
            }
        }

        public string[] AllArtists => NormalizedArtists.Concat(Artists).Distinct().ToArray();
    }
}