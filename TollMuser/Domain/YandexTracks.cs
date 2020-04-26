﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Tolltech.Muser.Models;
using Tolltech.YandexClient.ApiModels;

namespace Tolltech.Muser.Domain
{
    public class YandexTracks
    {
        private readonly Track[] tracks;

        public YandexTracks([NotNull] Track[] tracks)
        {
            this.tracks = tracks;
        }

        [CanBeNull]
        public Track FindEqualTrack(NormalizedTrack normalizedTrack)
        {
            return tracks.FirstOrDefault(yaTrack =>
                yaTrack.Albums.Length > 0
                && BrutteEquals(yaTrack.Title, normalizedTrack.Title)
                && yaTrack.Artists.Any(y => normalizedTrack.AllArtists.Any(v =>
                {
                    var yLower = y.Name.ToLowerInvariant();
                    var vLower = v.ToLowerInvariant();
                    return OneContainsAnother(vLower, yLower) ||
                           OneContainsAnother(BrutteNormalize(vLower), BrutteNormalize(yLower));
                })));
        }

        private static bool OneContainsAnother(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
            {
                return false;
            }

            return left.Contains(right) || right.Contains(left);
        }

        private static string BrutteNormalize(string src)
        {
            return src.NormalizeTrackInfo().ToOnlyLetterString().ReplacePossibleErrorChars();
        }

        private static bool BrutteEquals(string left, string right)
        {
            if (string.Equals(left, right, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            var leftBrutteNormalize = BrutteNormalize(left);
            return !string.IsNullOrWhiteSpace(leftBrutteNormalize)
                   && leftBrutteNormalize == BrutteNormalize(right);
        }
    }
}