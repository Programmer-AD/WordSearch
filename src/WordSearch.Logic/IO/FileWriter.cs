using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileWriter : IFileWriter
    {
        public Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(string value)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(Span<byte> bytes)
        {
            throw new NotImplementedException();
        }
    }
}
