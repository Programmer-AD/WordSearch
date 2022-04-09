using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.Interfaces
{
    internal interface IDatabaseFactory
    {
        Task<IDatabase> MakeDatabaseAsync(string dbName, IFileIO charsFile, IFileIO wordsFile);
    }
}
