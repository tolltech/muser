using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tolltech.Muser.Domain;
using Tolltech.MuserUI.Common;
using Tolltech.MuserUI.Models.Sync;

namespace Tolltech.MuserUI.Sync
{
    public class TrackGetter : ITrackGetter
    {
        private readonly IVkService vkService;

        public TrackGetter(IVkService vkService)
        {
            this.vkService = vkService;
        }

        public async Task<TracksModel> GetTracksAsync(string source)
        {
            var vkId = GetVkId(source);
            if (!string.IsNullOrWhiteSpace(vkId))
            {
                return (await vkService.GetVkTracksUnauthorizedAsync(vkId).ConfigureAwait(false)).ToTracksModel();
            }

            var lines = source.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<TrackModel>(lines.Length);
            foreach (var line in lines)
            {
                var splits = line.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                    .ToArray();
                if (splits.Length != 2)
                {
                    continue;
                }

                result.Add(new TrackModel
                {
                    Artist = splits[0],
                    Title = splits[1]
                });
            }

            return new TracksModel
            {
                Tracks = result.ToList()
            };
        }

        public string GetAudioUrl(string source)
        {
            return GetVkId(source);
        }

        private string GetVkId(string source)
        {
            if (int.TryParse(source.Trim(), out var result))
            {
                return result.ToString();
            }

            if (!Uri.IsWellFormedUriString(source, UriKind.Absolute))
            {
                return null;
            }

            var uri = new Uri(source);
            var path = uri.AbsolutePath.Trim(' ', '/', '\\');
            var endOfPath = new string(path.Reverse().TakeWhile(x => char.IsDigit(x) || x == '-').Reverse().ToArray());

            if (int.TryParse(endOfPath, out var result2))
            {
                return result2.ToString();
            }

            return null;
        }
    }
}