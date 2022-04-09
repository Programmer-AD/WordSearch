namespace WordSearch.Logic.Interfaces
{
    public interface IDatabaseManager
    {
        Task CreateAsync(string dbName, string chars);
        Task<IDatabase> GetAsync(string dbName);
        Task DeleteAsync(string dbName);
        Task<bool> Exists(string dbName);
        Task<IEnumerable<string>> GetDbNamesAsync();
    }
}
