namespace WordSearch.Logic.Exceptions.WordsFile
{
    public class WordAlreadyExistsException : Exception
    {
        public WordAlreadyExistsException(string word) : base(GetMessageByWord(word))
        {
        }

        private static string GetMessageByWord(string word)
        {
            return $"Word \"{word}\" already exists in current database";
        }
    }
}
