namespace WordSearch.Logic.Exceptions.WordsFile
{
    internal class WordNotFoundException : Exception
    {
        public WordNotFoundException(string word) : base(GetMessageByWord(word))
        {
        }

        private static string GetMessageByWord(string word)
        {
            return $"Word \"{word}\" not found in current database";
        }
    }
}
