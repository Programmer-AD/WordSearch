using WordSearch.Logic.Interfaces.Encoders;

namespace WordSearch.Logic.Encoders
{
    internal class WordEncoderFactory : IWordEncoderFactory
    {
        public IWordEncoder MakeWordEncoder(string chars)
        {
            return new WordEncoder(chars);
        }
    }
}
