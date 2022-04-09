using System.Xml.Linq;

namespace WordSearch.Logic.Exceptions.Database
{
    public class DatabaseNotFoundException : Exception
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
