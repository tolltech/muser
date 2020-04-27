using System;
using System.Linq;
using Tolltech.Muser.Models;

namespace Tolltech.Muser.Domain
{
    public class SimpleLinesTrackGetter : ISimpleLinesTrackGetter
    {
        public int Order => 100;

        public bool TryParseText(string text, out SourceTrack[] tracks)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                tracks = Array.Empty<SourceTrack>();
            }
            else
            {
                tracks = text.Split(new[] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries).Select(line =>
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

            return true;
        }
    }
}