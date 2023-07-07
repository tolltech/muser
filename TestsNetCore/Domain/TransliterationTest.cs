using FluentAssertions;
using NUnit.Framework;
using WebApp.Infrastructure;

namespace Tolltech.TestsNetCore.Domain
{
    public class TransliterationTest : TestBase
    {
        [Test]
        [TestCase(@"абвгдеёжзийклмнопрстуфхцчшщъыьэюя",
                  @"abvgdeyozhziyklmnoprstufhcchshshhyeyuya")]
        [TestCase(@"АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ",
                  @"ABVGDEYOZHZIYKLMNOPRSTUFHCCHSHSHHYEYUYA")]
        public void TestTransliterate(string input, string expected)
        {
            Transliteration.Front(input).Should().Be(expected);
        }
    }
}