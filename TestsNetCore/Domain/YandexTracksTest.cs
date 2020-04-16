﻿using System.Linq;
using NUnit.Framework;
using Tolltech.Muser.Domain;
using Tolltech.Muser.Models;
using Tolltech.YandexClient.ApiModels;

namespace Tolltech.TestsNetCore.Domain
{
    public class YandexTracksTest : TestBase
    {
        [Test, TestCaseSource(nameof(TestCases))]
        public void TestGetTracks(VkTrack vkTrack, Track yaTrack, bool expected, string msg = null)
        {
            var track = new YandexTracks(new[] {yaTrack}).FindEqualTrack(vkTrack);
            if (expected)
            {
                Assert.AreEqual(yaTrack, track, GetMessage(vkTrack, yaTrack, msg));
            }
            else
            {
                Assert.AreNotEqual(yaTrack, track, GetMessage(vkTrack, yaTrack, msg));
            }
        }

        private string GetMessage(VkTrack vkTrack, Track yaTrack, string msg)
        {
            return
                $"#{msg}: {vkTrack.Artist}-{vkTrack.Title} vs {string.Join(", ", yaTrack.Artists.Select(x => x.Name))}-{yaTrack.Title}";
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

        static VkTrack CreateVkTrack(string title, string artist) => new VkTrack {Artist = artist, Title = title};

        static Track CreateYaTrack(string title, params string[] artists) => new Track
        {
            Artists = artists.Select(artist => new Artist {Name = artist}).ToArray(), Title = title,
            Albums = new[] {new Album()}
        };
    }
}