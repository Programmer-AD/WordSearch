using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.Interfaces.Primary
{
    public interface IDatabaseFactory
    {
        IDatabase MakeDatabase(string dbName, IFileIO charsFileIO, IFileIO wordsFileIO);
    }
}
