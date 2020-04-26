using System;
using NUnit.Framework;
using Tolltech.Muser.Models;

namespace Tolltech.TestsNetCore.Domain
{
    public class VkTrackTest : TestBase
    {
        [Test, TestCaseSource(nameof(TestCases))]
        public void TestArtists(string artist, string[] expected)
        {
            CollectionAssert.AreEqual(expected, new NormalizedTrack {Artist = artist}.Artists);
            CollectionAssert.AreEqual(expected, new NormalizedTrack {Artist = artist}.NormalizedArtists);
        }

        [Test, TestCaseSource(nameof(TestCasesForNormalize))]
        public void TestWithNormalizedArtists(string artist, string[] expected)
        {
            CollectionAssert.AreEqual(expected, new NormalizedTrack {Artist = artist}.AllArtists);
        }

        private static readonly object[] TestCases =
        {
            new object[] {"", Array.Empty<string>()},
            new object[] {"art", new[] {"art"}},
            new object[] {"art&art2", new[] {"art", "art2"}},
            new object[] {"art ft art2", new[] {"art", "art2"}},
            new object[] {"art ft. art2", new[] {"art", "art2"}},
            new object[] {"art ft.art2", new[] {"art", "art2"}},
            new object[] {"art ft.art2", new[] {"art", "art2"}},
            new object[] {"art,art2,art3", new[] {"art", "art2", "art3"}},
        };

        private static readonly object[] TestCasesForNormalize =
        {
            new object[] {"aRt ft. art2(lyric)", new[] {"art", "art2", "aRt", "art2(lyric)" } },
            new object[] {"aRt ft. art2<lyric>", new[] {"art", "art2", "aRt", "art2<lyric>" } },
            new object[] {"art[text] ft.art2", new[] {"art", "art2", "art[text]" } },
            new object[] {"[text]", new[] {"[text]"}},
            new object[] {"[text] (lyrics)", new[] {"[text] (lyrics)"}},
            new object[] {"text (lyrics)", new[] {"text", "text (lyrics)" } },
        };
    }
}