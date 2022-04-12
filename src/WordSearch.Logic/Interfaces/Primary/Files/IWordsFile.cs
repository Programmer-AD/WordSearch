namespace WordSearch.Logic.Interfaces.Primary.Files
{
    public interface IWordsFile : IEnumerable<string>
    {
        public string Chars { get; }

        public long Add(string word);
        public void Delete(long position);
        public long GetWordPosition(string word);
        public string GetWord(long position);
    }
}
