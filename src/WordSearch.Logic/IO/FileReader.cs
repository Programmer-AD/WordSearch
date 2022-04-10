using System.Text;
using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileReader : IFileReader
    {
        private readonly Stream stream;
        private readonly BinaryReader reader;

        public FileReader(Stream stream, Encoding encoding)
        {
            this.stream = stream;
            reader = new BinaryReader(stream, encoding);
        }

        public async Task GetBytesAsync(byte[] bytes)
        {
            await stream.ReadAsync(bytes);
        }

        public async Task<string> GetStringAsync()
        {
            var result = await Task.FromResult(reader.ReadString());
            return result;
        }
    }
}
