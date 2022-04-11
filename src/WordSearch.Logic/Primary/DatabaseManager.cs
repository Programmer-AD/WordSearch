﻿using Microsoft.Extensions.Options;
using WordSearch.Logic.Exceptions.DatabaseManager;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.Logic.Primary
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
            IOptions<DatabaseConfig> config = null)
        {
            this.config = config.Value ?? DatabaseConstants.DefaultConfig;
            this.fileManager = fileManager;
            this.fileIOFactory = fileIOFactory;
            this.databaseFactory = databaseFactory;

            EnsureDirectoryCreated();
        }

        public async Task Create(string dbName, string chars)
        {
            CheckChars(chars);

            await CheckDbExistence(dbName, shouldExist: false);

            var wordsFilePath = GetWordsFilePath(dbName);
            fileManager.Create(wordsFilePath);
            var wordsFile = fileIOFactory.MakeFileIO(wordsFilePath);
            await wordsFile.Writer.Write(chars);
            await wordsFile.Writer.Flush();

            var charsFilePath = GetCharsFilePath(dbName);
            fileManager.Create(charsFilePath);
        }

        public async Task Delete(string dbName)
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

        public async Task<IDatabase> Get(string dbName)
        {
            await CheckDbExistence(dbName, shouldExist: true);

            var charsFilePath = GetCharsFilePath(dbName);
            var charsFileIO = fileIOFactory.MakeFileIO(charsFilePath);

            var wordsFilePath = GetWordsFilePath(dbName);
            var wordsFileIO = fileIOFactory.MakeFileIO(wordsFilePath);

            var database = databaseFactory.MakeDatabase(dbName, charsFileIO, wordsFileIO);
            return database;
        }

        public async Task<bool> Exists(string dbName)
        {
            CheckDbName(dbName);
            var wordsFilePath = GetWordsFilePath(dbName);
            var result = fileManager.Exists(wordsFilePath);
            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<string>> GetDbNames()
        {
            var fileFormat = DatabaseConstants.DatabaseWordFileExtension;
            var dbNames = fileManager.GetDirectoryFiles(config.DatabaseDirectoryPath)
                .Where(x => x.EndsWith(fileFormat))
                .Select(x => Path.GetFileName(x[..^fileFormat.Length]));

            return await Task.FromResult(dbNames);
        }

        private void EnsureDirectoryCreated()
        {
            var directoryPath = config.DatabaseDirectoryPath;
            if (!fileManager.DirectoryExists(directoryPath))
            {
                fileManager.CreateDirectory(directoryPath);
            }
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
            return Path.Combine(config.DatabaseDirectoryPath, fileName);
        }

        private static void CheckChars(string chars)
        {
            if (string.IsNullOrEmpty(chars))
            {
                throw new ArgumentException("Chars must not be null or empty", nameof(chars));
            }
            var charSet = chars.ToHashSet();
            if (charSet.Count < chars.Length)
            {
                throw new ArgumentException("Chars must not contain same character twice", nameof(chars));
            }
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
            var dbExists = await Exists(dbName);
            if (dbExists != shouldExist)
            {
                throw shouldExist ?
                    new DatabaseNotFoundException(dbName) :
                    new DatabaseAlreadyExistsException(dbName);
            }
        }
    }
}
