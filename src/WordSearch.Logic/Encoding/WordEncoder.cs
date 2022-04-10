using WordSearch.Logic.Interfaces.Encoding;

namespace WordSearch.Logic.Encoding
{
    internal class WordEncoder : IWordEncoder
    {
        public string Chars { get; }

        public WordEncoder(string chars)
        {
            Chars = chars;
        }

        public byte[] GetCharCounts(string word)
        {
            throw new NotImplementedException();
        }
    }
}
