using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileReader : IFileReader
    {
        public Task GetBytesAsync(out Span<byte> bytes)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStringAsync()
        {
            throw new NotImplementedException();
        }
    }
}
