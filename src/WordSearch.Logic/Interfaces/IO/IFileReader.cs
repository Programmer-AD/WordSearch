namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileReader
    {
        long Position { get; set; }

        Task<string> GetStringAsync();
        Task GetBytesAsync(out Span<byte> bytes);
    }
}
