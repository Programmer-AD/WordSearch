namespace WordSearch.Logic.Exceptions.DatabaseManager
{
    public class DatabaseWrongNameException : Exception
    {
        public DatabaseWrongNameException(string dbName, string reason)
            : base(GetMessageByDbName(dbName, reason))
        {
        }

        private static string GetMessageByDbName(string dbName, string reason)
        {
            return $"Database name \"{dbName}\" is incorrect! Reason: {reason}";
        }
    }
}
