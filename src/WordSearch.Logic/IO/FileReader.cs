using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileReader : IFileReader
    {
        public Task GetBytesAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetStringAsync()
        {
            throw new NotImplementedException();
        }
    }
}
