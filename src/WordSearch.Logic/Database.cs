using WordSearch.Logic.Interfaces;
using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic
{
    internal class Database : IDatabase
    {
        private readonly IFileIO charsFile;
        private readonly IFileIO wordsFile;

        public string Name { get; }
        public string Chars { get; private set; }

        public Database(string name, IFileIO charsFile, IFileIO wordsFile)
        {
            Name = name;
            this.charsFile = charsFile;
            this.wordsFile = wordsFile;
        }

        public Task Init()
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(string word)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string word)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetWordsAsync(string word, byte maxDifference)
        {
            throw new NotImplementedException();
        }
    }
}
