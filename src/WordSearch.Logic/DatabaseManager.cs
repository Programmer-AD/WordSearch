using WordSearch.Logic.Exceptions.Database;
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
            await CheckDbExistence(dbName, shouldExist: false);

            var wordsFilePath = GetWordsFilePath(dbName);
            fileManager.Create(wordsFilePath);
            var wordsFile = fileIOFactory.MakeFileIO(wordsFilePath);
            await wordsFile.Writer.WriteAsync(chars);

            var charsFilePath = GetCharsFilePath(dbName);
            fileManager.Create(charsFilePath);
            var charsFile = fileIOFactory.MakeFileIO(charsFilePath);
            await charsFile.Writer.WriteAsync(chars);
        }

        public async Task DeleteAsync(string dbName)
        {
            await CheckDbExistence(dbName, shouldExist: true);

            var charsFilePath = GetCharsFilePath(dbName);
            if (fileManager.Exists(charsFilePath))
            {
                fileManager.Delete(charsFilePath);
            }

            var wordsFilePath = GetWordsFilePath(dbName);
            fileManager.Delete(wordsFilePath);
        }

        public async Task<IDatabase> GetAsync(string dbName)
        {
            await CheckDbExistence(dbName, shouldExist: true);

            var charsFilePath = GetCharsFilePath(dbName);
            var charsFile = fileIOFactory.MakeFileIO(charsFilePath);

            var wordsFilePath = GetWordsFilePath(dbName);
            var wordsFile = fileIOFactory.MakeFileIO(wordsFilePath);

            var database = await databaseFactory.MakeDatabaseAsync(dbName, charsFile, wordsFile);
            return database;
        }

        public async Task<bool> ExistsAsync(string dbName)
        {
            CheckDbName(dbName);
            var wordsFilePath = GetWordsFilePath(dbName);
            var result = fileManager.Exists(wordsFilePath);
            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<string>> GetDbNamesAsync()
        {
            var fileFormat = DatabaseConstants.DatabaseWordFileExtension;
            var dbNames = fileManager.GetDirectoryFiles(config.DatabaseDirectory)
                .Where(x => x.EndsWith(fileFormat))
                .Select(x => x[..^fileFormat.Length]);

            return await Task.FromResult(dbNames);
        }

        internal string GetCharsFilePath(string dbName)
        {
            var fileName = $"{dbName}{DatabaseConstants.DatabaseCharsFileExtension}";
            return GetDatabasePath(fileName);
        }

        internal string GetWordsFilePath(string dbName)
        {
            var fileName = $"{dbName}{DatabaseConstants.DatabaseWordFileExtension}";
            return GetDatabasePath(fileName);
        }

        private string GetDatabasePath(string fileName)
        {
            return Path.Combine(config.DatabaseDirectory, fileName);
        }

        private static void CheckDbName(string dbName)
        {
            if (string.IsNullOrEmpty(dbName))
            {
                throw new DatabaseWrongNameException(dbName, "Cant be null or empty");
            }
            else
            {
                var match = DatabaseConstants.CorrectDatabaseNameRegex.Match(dbName);
                var notFullMatch = match?.Length != dbName.Length;
                if (notFullMatch)
                {
                    throw new DatabaseWrongNameException(dbName, "Contains invalid characters");
                }
            }
        }

        private async Task CheckDbExistence(string dbName, bool shouldExist)
        {
            var dbExists = await ExistsAsync(dbName);
            if (dbExists != shouldExist)
            {
                throw shouldExist ? 
                    new DatabaseNotFoundException(dbName) :
                    new DatabaseAlreadyExistsException(dbName);
            }
        }
    }
}
