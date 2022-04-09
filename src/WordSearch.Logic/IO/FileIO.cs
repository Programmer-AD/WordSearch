using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileIO : IFileIO
    {
        public long StreamPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public long StreamLength => throw new NotImplementedException();

        public IFileReader Reader { get; }

        public IFileWriter Writer { get; }
    }
}
