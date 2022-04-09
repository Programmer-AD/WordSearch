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

        public void GetBytes(string word, out Span<byte> bytes)
        {
            throw new NotImplementedException();
        }
    }
}
