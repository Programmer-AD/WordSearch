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

        public int GetBytes(Memory<byte> bytes)
        {
            var readed = stream.Read(bytes.Span);
            return readed;
        }

        public string GetString()
        {
            var result = reader.ReadString();
            return result;
        }
    }
}
