namespace WordSearch.Logic.Interfaces.Primary
{
    public interface IDatabaseManager
    {
        void Create(string dbName, string chars);
        IDatabase Get(string dbName);
        void Delete(string dbName);
        bool Exists(string dbName);
        IEnumerable<string> GetDbNames();
    }
}
