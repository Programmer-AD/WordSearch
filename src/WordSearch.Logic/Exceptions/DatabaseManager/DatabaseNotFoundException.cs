namespace WordSearch.Logic.Exceptions.DatabaseManager
{
    public class DatabaseNotFoundException : DatabaseException
    {
        public DatabaseNotFoundException(string dbName)
            : base(GetMessageByDbName(dbName))
        {
        }

        private static string GetMessageByDbName(string dbName)
        {
            return $"Database with name \"{dbName}\" not found!";
        }
    }
}
