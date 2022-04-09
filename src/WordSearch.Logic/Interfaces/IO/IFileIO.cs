namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileIO
    {
        long StreamPosition { get; set; }
        long StreamLength { get; }

        IFileReader Reader { get; }
        IFileWriter Writer { get; }
    }
}
