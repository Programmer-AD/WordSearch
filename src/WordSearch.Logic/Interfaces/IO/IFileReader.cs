namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileReader
    {
        string GetString();
        int GetBytes(Memory<byte> bytes);
    }
}
