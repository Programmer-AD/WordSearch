using WordSearch.Logic.Interfaces;
using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic
{
    internal class DatabaseFactory : IDatabaseFactory
    {
        public async Task<IDatabase> MakeDatabaseAsync(string dbName, IFileIO charsFile, IFileIO wordsFile)
        {
            var database = new Database(dbName, charsFile, wordsFile);
            await database.Init();
            return database;
        }
    }
}
