namespace WordSearch.Logic
{
    internal static class DatabaseConstants
    {
        public const string DatabaseCharsFileFormat = "chars.ws";
        public const string DatabaseWordFileFormat = "word.ws";

        public static readonly DatabaseConfig DefaultConfig = new()
        {
            DatabaseDirectory = "./dbs"
        };
    }
}
