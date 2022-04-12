namespace WordSearch.Logic.Exceptions.WordsFile
{
    public class WrongWordException : WordException
    {
        public WrongWordException(string word, string reason) : base(GetMessageByWord(word, reason))
        {
        }

        private static string GetMessageByWord(string word, string reason)
        {
            return $"Word \"{word}\" is wrong. Reason: {reason}";
        }
    }
}
