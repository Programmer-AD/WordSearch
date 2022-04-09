using WordSearch.Logic.Interfaces.Encoding;

namespace WordSearch.Logic.Encoding
{
    internal class WordEncoderFactory : IWordEncoderFactory
    {
        public IWordEncoder MakeWordEncoder(string chars)
        {
            return new WordEncoder(chars);
        }
    }
}
