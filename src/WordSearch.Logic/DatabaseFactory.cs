using WordSearch.Logic.Interfaces;
using WordSearch.Logic.Interfaces.Encoders;
using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic
{
    internal class DatabaseFactory : IDatabaseFactory
    {
        private readonly IWordEncoderFactory wordEncoderFactory;

        public DatabaseFactory(IWordEncoderFactory wordEncoderFactory)
        {
            this.wordEncoderFactory = wordEncoderFactory;
        }

        public async Task<IDatabase> MakeDatabaseAsync(string dbName, IFileIO charsFile, IFileIO wordsFile)
        {
            var wordEncoder = await GetWordEncoder(wordsFile);
            var database = new Database(dbName, charsFile, wordsFile, wordEncoder);
            return database;
        }

        private async Task<IWordEncoder> GetWordEncoder(IFileIO wordsFile)
        {
            wordsFile.StreamPosition = 0;
            var chars = await wordsFile.Reader.GetStringAsync();
            var wordEncoder = wordEncoderFactory.MakeWordEncoder(chars);
            return wordEncoder;
        }
    }
}
