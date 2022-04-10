namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileWriter
    {
        Task WriteAsync(string value);
        Task WriteAsync(byte[] bytes);

        Task FlushAsync();
    }
}
