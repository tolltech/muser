﻿using System.Linq;
using NUnit.Framework;
using Tolltech.Muser.Domain;
using Tolltech.Muser.Models;
using Tolltech.SpotifyClient.ApiModels;

namespace Tolltech.TestsNetCore.Domain
{
    public class YandexTracksTest : TestBase
    {
        [Test, TestCaseSource(nameof(TestCases))]
        public void TestGetTracks(NormalizedTrack normalizedTrack, Track yaTrack, bool expected, string msg = null)
        {
            var track = new YandexTracks(new[] {yaTrack}).FindEqualTrack(normalizedTrack);
            if (expected)
            {
                Assert.AreEqual(yaTrack, track, GetMessage(normalizedTrack, yaTrack, msg));
            }
            else
            {
                Assert.AreNotEqual(yaTrack, track, GetMessage(normalizedTrack, yaTrack, msg));
            }
        }

        private string GetMessage(NormalizedTrack normalizedTrack, Track yaTrack, string msg)
        {
            return
                $"#{msg}: {normalizedTrack.Artist}-{normalizedTrack.Title} vs {string.Join(", ", yaTrack.Artists.Select(x => x.Name))}-{yaTrack.Title}";
        }

        static readonly object[] TestCases =
        {
            new object[] {CreateVkTrack("title", "aRtist"), CreateYaTrack("title", "artist"), true, "1"},
            new object[] {CreateVkTrack("titleЁсСlli", "aRtist"), CreateYaTrack("titleеcly", "artist"), true, "16"},
            new object[] {CreateVkTrack("title ", "artist"), CreateYaTrack(" Title", "artist"), true, "13"},
            new object[] {CreateVkTrack("title", "artist"), CreateYaTrack("title", "aRtists"), true, "2"},
            new object[] {CreateVkTrack("title", "artist2 ft artist"), CreateYaTrack("title", "artist"), true, "3"},
            new object[] {CreateVkTrack("title", "artist2 ft. artist"), CreateYaTrack("title", "artist"), true, "4"},
            new object[] {CreateVkTrack("title", "artist2 & artist"), CreateYaTrack("title", "artist3", "artist"), true, "5"},
            new object[] {CreateVkTrack("title", "artist2&artist"), CreateYaTrack("title", "artist"), true, "6"},
            new object[] {CreateVkTrack("title", "artist2, artist"), CreateYaTrack("title", "artist"), true, "7"},
            new object[] {CreateVkTrack("title", "artists"), CreateYaTrack("title", "artist"), true, "8"},
            new object[] {CreateVkTrack("title", "artist"), CreateYaTrack("titlef", "artist"), false, "9"},
            new object[] {CreateVkTrack("titlef", "artist"), CreateYaTrack("title", "artist"), false, "10"},
            new object[] {CreateVkTrack("title", "artist"), CreateYaTrack("title2#", "artist"), true, "14"},
            new object[] {CreateVkTrack("title2#", "artist"), CreateYaTrack("title", "artist"), true, "15"},
            new object[] {CreateVkTrack(null, "artist"), CreateYaTrack("title", "artist"), false, "11"},
            new object[] {CreateVkTrack("title", "artist"), CreateYaTrack(null, "artist"), false, "12"},
        };

        static NormalizedTrack CreateVkTrack(string title, string artist) => new NormalizedTrack {Artist = artist, Title = title};

        static Track CreateYaTrack(string title, params string[] artists) => new Track
        {
            Artists = artists.Select(artist => new Artist { Name = artist }).ToArray(), Title = title,
            Album = new Album()
        };
    }
}