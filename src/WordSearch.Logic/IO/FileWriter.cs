using System.Text;
using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.IO
{
    internal class FileWriter : IFileWriter
    {
        private readonly Stream stream;
        private readonly BinaryWriter writer;

        public FileWriter(Stream stream, Encoding encoding)
        {
            this.stream = stream;
            writer = new BinaryWriter(stream, encoding);
        }

        public async Task FlushAsync()
        {
            stream.Flush();
            await Task.CompletedTask;
        }

        public async Task WriteAsync(string value)
        {
            writer.Write(value);
            await Task.CompletedTask;
        }

        public async Task WriteAsync(ReadOnlyMemory<byte> bytes)
        {
            stream.Write(bytes.Span);
            await Task.CompletedTask;
        }
    }
}
