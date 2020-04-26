using System;
using System.Linq;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public class TrackGetter : ITrackGetter
    {
        public SourceTrack[] GetTracks(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return Array.Empty<SourceTrack>();
            }

            return source.Split(new[] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries).Select(line =>
                {
                    var splits = line.Split(new[] {" - "}, StringSplitOptions.RemoveEmptyEntries);

                    if (splits.Length == 0)
                    {
                        return null;
                    }

                    if (splits.Length == 1)
                    {
                        return new SourceTrack
                        {
                            Artist = string.Empty,
                            Title = splits[0]
                        };
                    }

                    return new SourceTrack
                    {
                        Artist = splits[0],
                        Title = string.Join(" ", splits.Skip(1))
                    };
                })
                .Where(x => x != null)
                .ToArray();
        }
    }
}