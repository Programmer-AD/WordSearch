namespace WordSearch.Logic.Interfaces
{
    public interface IDatabaseManager
    {
        Task CreateAsync(string name, string chars);
        Task<IDatabase> GetAsync(string name);
        Task DeleteAsync(string name);
    }
}
