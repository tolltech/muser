﻿using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Tolltech.Muser.Domain
{
    public static class StringExtensions
    {
        private const char russianCChar = 'с';
        private const char englishCChar = 'c';

        private static readonly Regex[] regexes = new[]
        {
            new Regex(@"\[.*\]"),
            new Regex(@"\(.*\)"),
            new Regex(@"\<.*\>")
        };

        [CanBeNull]
        public static string NormalizeTrackInfo([CanBeNull] this string src)
        {
            if (src == null)
            {
                return null;
            }

            var s = src;
            foreach (var regex in regexes)
            {
                s = regex.Replace(s, string.Empty);
            }

            return s.Trim().ToLowerInvariant();
        }

        [CanBeNull]
        public static string NormalizeForYandex([CanBeNull] this string src)
        {
            return src?.Replace("#", string.Empty).Replace("&amp;", string.Empty);
        }

        [CanBeNull]
        public static string ToOnlyLetterString([CanBeNull] this string src)
        {
            return src == null ? null : new string(src.Where(char.IsLetter).ToArray());
        }

        [CanBeNull]
        public static string ReplacePossibleErrorChars([CanBeNull] this string src)
        {
            return src?.Replace('ё', 'е')
                    .Replace("ll", "l")
                    .Replace("нн", "н")
                    .Replace("&amp;", string.Empty)
                    .Replace("zz", "z")
                    .Replace("дж", "j")
                    .Replace("dzh", "j")
                    .Replace("qu", "kv")
                    .Replace("ea", "i")
                    .Replace("yy", "i")
                    .Replace("iy", "i")
                    .Replace("yi", "i")
                    .Replace("ii", "i")
                    .Replace("ya", "a")
                    .Replace("ts", "c")
                    .Replace("oo", "u")
                    .Replace("x", "ks")
                    .Replace(russianCChar, englishCChar)
                    .Replace('s', englishCChar)
                    .Replace("cc", "c")
                    .Replace('y', 'i')
                    .Replace('q', 'k')
                    
                ;
        }
    }
}