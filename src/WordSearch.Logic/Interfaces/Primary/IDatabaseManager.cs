namespace WordSearch.Logic.Interfaces.Primary
{
    public interface IDatabaseManager
    {
        Task CreateAsync(string dbName, string chars);
        Task<IDatabase> GetAsync(string dbName);
        Task DeleteAsync(string dbName);
        Task<bool> ExistsAsync(string dbName);
        Task<IEnumerable<string>> GetDbNamesAsync();
    }
}
