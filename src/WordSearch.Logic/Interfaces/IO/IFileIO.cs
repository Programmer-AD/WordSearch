namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileIO
    {
        IFileReader Reader { get; }
        IFileWriter Writer { get; }
    }
}
