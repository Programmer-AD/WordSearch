namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileWriter
    {
        void Write(string value);
        void Write(ReadOnlyMemory<byte> bytes);

        void Flush();
    }
}
