using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileIOFactory : IFileIOFactory
    {
        public IFileIO MakeFileIO(string filePath)
        {
            var stream = new FileStream(filePath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.Delete | FileShare.ReadWrite);

            var fileIO = new FileIO(stream);
            return fileIO;
        }
    }
}
