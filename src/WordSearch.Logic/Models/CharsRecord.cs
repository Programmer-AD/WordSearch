using System;

namespace WordSearch.Logic.Models
{
    internal struct CharsRecord
    {
        public byte[] CharCounts;
        public long WordPosition;

        public CharsRecord(byte[] buffer, int charCountLength)
        {
            if (buffer.Length < GetByteSize(charCountLength))
            {
                throw new ArgumentException("Buffer too small!", nameof(buffer));
            }
            CharCounts = new byte[charCountLength];

            using var memoryStream = new MemoryStream(buffer);
            using var reader = new BinaryReader(memoryStream);

            reader.Read(CharCounts);
            WordPosition = reader.ReadInt64();
        }

        public static int GetByteSize(int charCountLength)
        {
            return charCountLength + sizeof(long);
        }

        public int ByteSize => GetByteSize(CharCounts.Length);

        public void GetBytes(byte[] buffer)
        {
            if (buffer.Length < ByteSize)
            {
                throw new ArgumentException("Buffer too small!", nameof(buffer));
            }

            using var memoryStream = new MemoryStream(buffer);
            using var writer = new BinaryWriter(memoryStream);

            writer.Write(CharCounts);
            writer.Write(WordPosition);
            writer.Flush();
        }
    }
}
