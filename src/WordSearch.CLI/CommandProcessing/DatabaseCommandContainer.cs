using System.Text;
using WordSearch.Logic.Exceptions.DatabaseManager;
using WordSearch.Logic.Exceptions.WordsFile;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.CLI.CommandProcessing
{
    internal class DatabaseCommandContainer
    {
        private readonly IDatabaseManager databaseManager;

        public IDatabase UsedDatabase { get; private set; }

        public DatabaseCommandContainer(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        public string CreateDB(string dbName, string chars)
        {
            try
            {
                databaseManager.Create(dbName, chars);
                return $"Database \"{dbName}\" created successfully";
            }
            catch (DatabaseException e)
            {
                return e.Message;
            }
        }
        public string DeleteDB(string dbName)
        {
            try
            {
                databaseManager.Delete(dbName);
                if (UsedDatabase?.Name == dbName)
                {
                    UsedDatabase = null;
                }
                return $"Database \"{dbName}\" deleted successfully";
            }
            catch (DatabaseException e)
            {
                return e.Message;
            }
        }

        public string Use(string dbName)
        {
            try
            {
                UsedDatabase = databaseManager.Get(dbName);
                return $"Now using database \"{dbName}\"";
            }
            catch (DatabaseException e)
            {
                return e.Message;
            }
        }

        public string ShowDbs()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Databases: ")
                .AppendJoin("\r\n", databaseManager.GetDbNames().Select(x => $"- \"{x}\""))
                .AppendLine();

            return stringBuilder.ToString();
        }

        public string AddWord(string word)
        {
            if (UsedDatabase == null)
            {
                return $"No database selected";
            }
            try
            {
                UsedDatabase.Add(word);
                return $"Word \"{word}\" added successfully";
            }
            catch (WordException e)
            {
                return e.Message;
            }
        }

        public string DeleteWord(string word)
        {
            if (UsedDatabase == null)
            {
                return $"No database selected";
            }
            try
            {
                UsedDatabase.Delete(word);
                return $"Word \"{word}\" deleted successfully";
            }
            catch (WordException e)
            {
                return e.Message;
            }
        }

        public string FindWords(string word, byte maxDifference)
        {
            if (UsedDatabase == null)
            {
                return $"No database selected";
            }

            var result = UsedDatabase.GetWords(word, maxDifference);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Similar words: ")
                .AppendJoin("\r\n", result.Select(x => $"- \"{x}\""))
                .AppendLine();

            return stringBuilder.ToString();
        }

        public string ShowChars()
        {
            if (UsedDatabase == null)
            {
                return $"No database selected";
            }

            return $"Chars: {UsedDatabase.Chars}";
        }
    }
}
