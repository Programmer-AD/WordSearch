namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileWriter
    {
        long Position { get; set; }

        Task WriteAsync(string value);
        Task WriteAsync(Span<byte> bytes);
    }
}
