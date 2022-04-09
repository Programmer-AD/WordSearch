using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.Interfaces
{
    public interface IDatabaseFactory
    {
        Task<IDatabase> MakeDatabaseAsync(string dbName, IFileIO charsFile, IFileIO wordsFile);
    }
}
