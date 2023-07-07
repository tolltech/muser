using System.Linq;
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
            new object[] {CreateVkTrack("title", "король и шут"), CreateYaTrack("title", "korol i shut"), true, "17"},
            new object[] {CreateVkTrack("title", "найк борзов"), CreateYaTrack("title", "Naik Borzov"), true, "18"},
            new object[] {CreateVkTrack("title", "найк борзов"), CreateYaTrack("title", "Nayk Borzov"), true, "19"},
            new object[] {CreateVkTrack("title", "найк борзов"), CreateYaTrack("title", "Najk Borzov"), false, "20"},
            new object[] {CreateVkTrack("title", "Animal ДжаZ"), CreateYaTrack("title", "Animal Jazz"), true, "21"},
            new object[] {CreateVkTrack("title", "Ляпис Трубецкой"), CreateYaTrack("title", "Lyapis Trubetskoy"), true, "22"},
            new object[] {CreateVkTrack("Спокойной ночи", "Найк Борзов"), CreateYaTrack("Spokoynoy nochi", "Naik Borzov"), false, "23"},
            new object[] {CreateVkTrack("title", "Северный флот"), CreateYaTrack("title", "Severny Flot"), true, "24"},
            new object[] {CreateVkTrack("title", "Сплин"), CreateYaTrack("title", "Splean"), true, "25"},
            new object[] {CreateVkTrack("title", "Нейромонах Феофан"), CreateYaTrack("title", "Neuromonakh Feofan"), false, "25"},
            new object[] {CreateVkTrack("Не танцуй", "artis"), CreateYaTrack("Не танцуй - Аффинаж cover", "artist"), true, "26"},
            new object[] {CreateVkTrack("Вахтёрам", "Бумбокс"), CreateYaTrack("Вахтерам", "Boombox"), true, "27"},
            new object[] {CreateVkTrack("title", "Дельфин"), CreateYaTrack("title", "Dolphin"), false, "28"},
            new object[] {CreateVkTrack("LUV &amp; PAIN", "artist"), CreateYaTrack("LUV & PAIN", "artist"), true, "29"},
            new object[] {CreateVkTrack("title", "Блондинка КсЮ"), CreateYaTrack("title", "Blondinka KsU"), true, "30"},
            new object[] {CreateVkTrack("title", "Аквариум"), CreateYaTrack("title", "Aquarium"), true, "31"},
            new object[] {CreateVkTrack("title", "Сурганова и Оркестр"), CreateYaTrack("title", "Surganova & Orkestr"), true, "32"},
            new object[] {CreateVkTrack("Weak &amp; Tired", "title"), CreateYaTrack("Weak & Tired", "title"), true, "33"},
            new object[] {CreateVkTrack("title", "Алла Пугачёва"), CreateYaTrack("title", "Alla Pugacheva"), true, "34"},
        };

        static NormalizedTrack CreateVkTrack(string title, string artist) => new NormalizedTrack {Artist = artist, Title = title};

        static Track CreateYaTrack(string title, params string[] artists) => new Track
        {
            Artists = artists.Select(artist => new Artist { Name = artist }).ToArray(), Title = title,
            Album = new Album()
        };
    }
}