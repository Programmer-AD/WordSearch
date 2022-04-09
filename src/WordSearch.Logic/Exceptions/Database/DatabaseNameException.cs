namespace WordSearch.Logic.Exceptions.Database
{
    public class DatabaseNameException : Exception
    {
        public DatabaseNameException(string dbName, string reason)
            : base(GetMessageByDbName(dbName, reason))
        {
        }

        private static string GetMessageByDbName(string dbName, string reason)
        {
            return $"Database name \"{dbName}\" is incorrect! Reason: {reason}";
        }
    }
}
