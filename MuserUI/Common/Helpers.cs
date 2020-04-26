using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tolltech.Muser.Models;
using Tolltech.MuserUI.Models.Sync;
using Tolltech.YandexClient.ApiModels;

namespace Tolltech.MuserUI.Common
{
    public static class Helpers
    {
        public static Guid? SafeToGuid(this string src)
        {
            return Guid.TryParse(src, out var result) ? result : (Guid?)null;
        }

        public static bool IsNullOrWhitespace([CanBeNull] this string src)
        {
            return string.IsNullOrWhiteSpace(src);
        }

        [NotNull]
        public static string JoinToString<T>([NotNull] this IEnumerable<T> src, string separator = ", ")
        {
            return string.Join(separator, src);
        }

        [NotNull]
        public static TracksModel ToTracksModel([NotNull] this NormalizedTrack[] tracks)
        {
            return new TracksModel
            {
                Tracks = tracks
                    .Select(x => new TrackModel
                    {
                        Artist = x.Artist,
                        Title = x.Title
                    })
                    .ToList()
            };
        }

        [NotNull]
        public static TracksModel ToTracksModel([NotNull] this Track[] tracks)
        {
            return new TracksModel
            {
                Tracks = tracks
                    .Select(x => new TrackModel
                    {
                        Artist = x.Artists.Select(y => y.Name).JoinToString(),
                        Title = x.Title
                    })
                    .ToList()
            };
        }

        [NotNull]
        public static TracksModel ToTracksModel([NotNull] this SourceTrack[] tracks)
        {
            return new TracksModel
            {
                Tracks = tracks
                    .Select(x => new TrackModel
                    {
                        Artist = x.Artist,
                        Title = x.Title
                    })
                    .ToList()
            };
        }

        [NotNull]
        public static SourceTrack[] ToTracksModel(this TracksModel tracks)
        {
            return tracks?.Tracks?
                .Select(x =>
                    new SourceTrack
                    {
                        Title = x.Title,
                        Artist = x.Artist
                    })
                .ToArray() ?? Array.Empty<SourceTrack>();
        }
    }
}