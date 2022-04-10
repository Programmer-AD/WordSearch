using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary.Files;

namespace WordSearch.Logic.Primary.Files
{
    internal class WordsFile : IWordsFile
    {
        private readonly IFileIO wordsFileIO;

        public WordsFile(IFileIO wordsFileIO)
        {
            this.wordsFileIO = wordsFileIO;
        }

        public string Chars => throw new NotImplementedException();

        public Task<long> AddAsync(string word)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(long position)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetWordAsync(long position)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetWordPositionAsync(string word)
        {
            throw new NotImplementedException();
        }
    }
}
