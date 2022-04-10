namespace WordSearch.Logic.Primary
{
    public struct CharsRecord
    {
        private const int ConstFieldSize = sizeof(long);

        public Memory<byte> Bytes;

        public CharsRecord(Memory<byte> bytes)
        {
            Bytes = bytes;
        }

        public Memory<byte> CharCounts
        {
            get => Bytes[..^ConstFieldSize];
            set => value.CopyTo(Bytes);
        }

        internal Memory<byte> WordPositionBytes => Bytes[^ConstFieldSize..];
        public long WordPosition
        {
            get => BitConverter.ToInt64(WordPositionBytes.Span);
            set => BitConverter.TryWriteBytes(WordPositionBytes.Span, value);
        }

        public static int GetByteSize(int charCountLength)
        {
            return charCountLength + ConstFieldSize;
        }

        public int ByteSize => GetByteSize(CharCounts.Length);
    }
}
