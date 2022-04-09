namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileIOFactory
    {
        IFileIO MakeFileIO(string filePath);
    }
}
