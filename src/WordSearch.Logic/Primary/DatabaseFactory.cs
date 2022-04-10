using WordSearch.Logic.Interfaces.Encoders;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary;
using WordSearch.Logic.Primary.Files;

namespace WordSearch.Logic.Primary
{
    internal class DatabaseFactory : IDatabaseFactory
    {
        private readonly IWordEncoderFactory wordEncoderFactory;

        public DatabaseFactory(IWordEncoderFactory wordEncoderFactory)
        {
            this.wordEncoderFactory = wordEncoderFactory;
        }

        public IDatabase MakeDatabase(string dbName, IFileIO charsFileIO, IFileIO wordsFileIO)
        {
            var charsFile = new CharsFile(charsFileIO);
            var wordsFile = new WordsFile(wordsFileIO);
            var wordEncoder = wordEncoderFactory.MakeWordEncoder(wordsFile.Chars);
            var database = new Database(dbName, charsFile, wordsFile, wordEncoder);
            return database;
        }
    }
}
