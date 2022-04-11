using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileManager : IFileManager
    {
        public void Create(string filePath)
        {
            using var stream = File.Create(filePath);
        }

        public void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }

        public void Delete(string filePath)
        {
            File.Delete(filePath);
        }

        public bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public IEnumerable<string> GetDirectoryFiles(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath);
        }
    }
}
