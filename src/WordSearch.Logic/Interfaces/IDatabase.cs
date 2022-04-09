namespace WordSearch.Logic.Interfaces
{
    public interface IDatabase
    {
        string Chars { get; }

        Task AddAsync(string word);
        Task<IEnumerable<string>> GetWordsAsync(string word, byte maxDifference);
        Task DeleteAsync(string word);
    }
}
