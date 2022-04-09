namespace WordSearch.Logic.Interfaces.IO
{
    public interface IFileWriterFactory
    {
        IFileWriter CreateFileWriter(string fileName);
    }
}
