using WordSearch.Logic.Interfaces;
using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic
{
    internal class DatabaseManager : IDatabaseManager
    {
        private readonly IFileManager fileManager;
        private readonly IFileIOFactory fileIOFactory;
        private readonly IDatabaseFactory databaseFactory;
        private readonly DatabaseConfig config;

        public DatabaseManager(
            IFileManager fileManager,
            IFileIOFactory fileIOFactory,
            IDatabaseFactory databaseFactory,
            DatabaseConfig config = null)
        {
            this.config = config ?? DatabaseConstants.DefaultConfig;
            this.fileManager = fileManager;
            this.fileIOFactory = fileIOFactory;
            this.databaseFactory = databaseFactory;
        }

        public async Task CreateAsync(string dbName, string chars)
        {
            var wordsFilePath = GetWordsFilePath(dbName);
            fileManager.Create(wordsFilePath);

            var charsFilePath = GetCharsFilePath(dbName);
            fileManager.Create(charsFilePath);

            var charsFile = fileIOFactory.MakeFileIO(charsFilePath);
            await charsFile.Writer.WriteAsync(chars);
        }

        public async Task DeleteAsync(string dbName)
        {
            var wordsFilePath = GetWordsFilePath(dbName);
            fileManager.Delete(wordsFilePath);

            var charsFilePath = GetCharsFilePath(dbName);
            fileManager.Delete(charsFilePath);

            await Task.CompletedTask;
        }

        public async Task<IDatabase> GetAsync(string dbName)
        {
            var charsFilePath = GetCharsFilePath(dbName);
            var charsFile = fileIOFactory.MakeFileIO(charsFilePath);

            var wordsFilePath = GetWordsFilePath(dbName);
            var wordsFile = fileIOFactory.MakeFileIO(wordsFilePath);

            var database = await databaseFactory.MakeDatabaseAsync(dbName, charsFile, wordsFile);
            return database;
        }

        private string GetCharsFilePath(string dbName)
        {
            var fileName = $"{dbName}.{DatabaseConstants.DatabaseCharsFileFormat}";
            return GetDatabasePath(fileName);
        }

        private string GetWordsFilePath(string dbName)
        {
            var fileName = $"{dbName}.{DatabaseConstants.DatabaseWordFileFormat}";
            return GetDatabasePath(fileName);
        }

        private string GetDatabasePath(string fileName)
        {
            return Path.Combine(config.DatabaseDirectory, fileName);
        }
    }
}
