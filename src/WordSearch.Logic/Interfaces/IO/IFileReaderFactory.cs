namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileReaderFactory
    {
        IFileReader CreateFileReader(string fileName);
    }
}
