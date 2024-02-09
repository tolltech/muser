using System;
using System.IO;
using System.Linq;
using Tolltech.MuserUI.Common;

namespace Tolltech.MuserUI.Kontur.RandomText
{
    public class RandomTexter : IRandomTexter
    {
        public RandomTexter()
        {
            adjectives = File.ReadAllText("Data/Adjectives.txt")
                .Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
            
            nouns = File.ReadAllText("Data/Nouns.txt")
                .Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }
        
        public string GetRandomString()
        {
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var randomGod = random.Next(1, 11);

            if (randomGod >= 1 && randomGod < 3)
            {
                return $"{adjectives.GetRandomItem()} {nouns.GetRandomItem()}";
            }

            return nouns.GetRandomItem();
        }
        
        #region - Words -
        
        private readonly string[] adjectives;
        private readonly string[] nouns;

        #endregion
    }
}