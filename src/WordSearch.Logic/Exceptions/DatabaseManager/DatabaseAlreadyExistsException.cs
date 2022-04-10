namespace WordSearch.Logic.Exceptions.DatabaseManager
{
    public class DatabaseAlreadyExistsException : Exception
    {
        public DatabaseAlreadyExistsException(string dbName)
            : base(GetMessageByDbName(dbName))
        {
        }

        private static string GetMessageByDbName(string dbName)
        {
            return $"Database with name \"{dbName}\" already exists!";
        }
    }
}
