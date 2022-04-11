namespace WordSearch.Logic.Interfaces.Primary
{
    public interface IDatabase
    {
        string Name { get; }
        string Chars { get; }

        void Add(string word);
        IEnumerable<string> GetWords(string word, byte maxDifference);
        void Delete(string word);
    }
}
