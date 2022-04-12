namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileManager
    {
        void Create(string filePath);
        void CreateDirectory(string directoryPath);
        void Delete(string filePath);
        bool DirectoryExists(string directoryPath);
        bool Exists(string filePath);
        IEnumerable<string> GetDirectoryFiles(string directoryPath);
    }
}
