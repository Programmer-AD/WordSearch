namespace WordSearch.Logic.Interfaces.Primary.Files
{
    public interface IWordsFile : IAsyncEnumerable<string>
    {
        public string Chars { get; }

        public Task<long> AddAsync(string word);
        public Task DeleteAsync(long position);
        public Task<long> GetWordPositionAsync(string word);
        public Task<string> GetWordAsync(long position);
    }
}
