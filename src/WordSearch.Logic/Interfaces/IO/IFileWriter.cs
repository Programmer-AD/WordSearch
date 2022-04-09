namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileWriter
    {
        Task WriteAsync(string value);
        Task WriteAsync(Span<byte> bytes);

        Task FlushAsync();
    }
}
