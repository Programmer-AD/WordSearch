﻿using System.Text;
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

        public async Task<int> GetBytes(Memory<byte> bytes)
        {
            var readed = await Task.FromResult(reader.Read(bytes.Span));
            return readed;
        }

        public async Task<string> GetString()
        {
            var result = await Task.FromResult(reader.ReadString());
            return result;
        }
    }
}
