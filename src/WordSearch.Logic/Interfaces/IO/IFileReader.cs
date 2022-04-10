namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileReader
    {
        Task<string> GetStringAsync();
        Task GetBytesAsync(byte[] bytes);
    }
}
