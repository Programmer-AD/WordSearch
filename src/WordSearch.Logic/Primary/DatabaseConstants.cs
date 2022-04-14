using System.Text.RegularExpressions;

namespace WordSearch.Logic.Primary
{
    internal static class DatabaseConstants
    {
        public const string DatabaseCharsFileExtension = ".chars.ws";
        public const string DatabaseWordFileExtension = ".word.ws";
        public const int MaxWordLength = 127;

        public static readonly Regex CorrectDatabaseNameRegex = new("[A-Za-z0-9_\\-]+");

        public static readonly DatabaseConfig DefaultConfig = new()
        {
            DatabaseDirectoryPath = "./dbs"
        };
    }
}
