namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileReader
    {
        Task<string> GetStringAsync();
        Task<int> GetBytesAsync(Memory<byte> bytes);
    }
}
