using System.Reflection;
using System.Text;
using WordSearch.Logic.Exceptions.DatabaseManager;
using WordSearch.Logic.Exceptions.WordsFile;

namespace WordSearch.CLI
{
    internal partial class CommandProcessor
    {
        [CommandMethod]
        private string Help()
        {
            var stringBuilder = new StringBuilder();
            foreach (var overloads in commandMethods)
            {
                stringBuilder.AppendLine(overloads.Key);
                foreach (var method in overloads)
                {
                    stringBuilder.Append('\t').AppendLine(GetMethodDescription(method));
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }

        private static string GetMethodDescription(MethodInfo methodInfo)
        {
            var paramsDescription = string.Join(' ', methodInfo.GetParameters()
                .Select(x => $"{x.Name}:{x.ParameterType.Name}"));

            return $"{methodInfo.Name} {paramsDescription}";
        }

        [CommandMethod]
        private string CreateDB(string dbName, string chars)
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

        [CommandMethod]
        private string DeleteDB(string dbName)
        {
            try
            {
                databaseManager.Delete(dbName);
                if (UsedDatabase.Name == dbName)
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

        [CommandMethod]
        private string Use(string dbName)
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

        [CommandMethod]
        private string ShowDbs()
        {
            try
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Databases: ")
                    .AppendJoin("\r\n", databaseManager.GetDbNames().Select(x => $"- \"{x}\""))
                    .AppendLine();

                return stringBuilder.ToString();
            }
            catch (DatabaseException e)
            {
                return e.Message;
            }
        }


        [CommandMethod]
        private string AddWord(string word)
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

        [CommandMethod]
        private string DeleteWord(string word)
        {
            if (UsedDatabase == null)
            {
                return $"No database selected";
            }
            try
            {
                UsedDatabase.Add(word);
                return $"Word \"{word}\" deleted successfully";
            }
            catch (WordException e)
            {
                return e.Message;
            }
        }

        [CommandMethod]
        private string FindWords(string word, byte maxDifference)
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

        [CommandMethod]
        private string ShowChars()
        {
            if (UsedDatabase == null)
            {
                return $"No database selected";
            }

            return $"Chars: {UsedDatabase.Chars}";
        }
    }
}
