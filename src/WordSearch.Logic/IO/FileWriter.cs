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

        public void Flush()
        {
            stream.Flush();
        }

        public void Write(string value)
        {
            writer.Write(value);
        }

        public void Write(ReadOnlyMemory<byte> bytes)
        {
            stream.Write(bytes.Span);
        }
    }
}
