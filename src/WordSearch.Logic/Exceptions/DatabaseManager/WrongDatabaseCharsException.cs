namespace WordSearch.Logic.Exceptions.DatabaseManager
{
    public class WrongDatabaseCharsException : DatabaseException
    {
        public WrongDatabaseCharsException(string chars, string reason)
            : base(GetMessageByDbChars(chars, reason))
        {
        }

        private static string GetMessageByDbChars(string chars, string reason)
        {
            return $"Database chars \"{chars}\" is incorrect! Reason: {reason}";
        }
    }
}
