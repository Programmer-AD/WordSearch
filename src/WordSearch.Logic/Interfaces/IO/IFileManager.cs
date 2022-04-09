namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileManager
    {
        void Create(string filePath);
        void Delete(string filePath);
        bool Exists(string filePath);
        IEnumerable<string> GetDirectoryFiles(string directoryPath);
    }
}
