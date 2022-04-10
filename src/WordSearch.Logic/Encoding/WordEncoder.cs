using WordSearch.Logic.Interfaces.Encoding;

namespace WordSearch.Logic.Encoding
{
    internal class WordEncoder : IWordEncoder
    {
        private readonly IReadOnlyDictionary<char, int> charIndexes;

        public string Chars { get; }

        public WordEncoder(string chars)
        {
            Chars = chars;
            charIndexes = GetCharIndexes();
        }

        public byte[] GetCharCounts(string word)
        {
            var result = new byte[Chars.Length];
            foreach (var @char in word)
            {
                if (charIndexes.TryGetValue(@char, out var index))
                {
                    result[index]++;
                }
            }
            return result;
        }

        private IReadOnlyDictionary<char, int> GetCharIndexes()
        {
            return Chars.Select((@char, pos) => (@char, pos)).ToDictionary(x => x.@char, x => x.pos);
        }
    }
}
