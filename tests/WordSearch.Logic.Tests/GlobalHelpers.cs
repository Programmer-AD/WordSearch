using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.Tests
{
    internal static class GlobalHelpers
    {
        public static FileIOMock MockFileIO()
        {
            var fileIOMock = new Mock<IFileIO>();
            var fileReaderMock = new Mock<IFileReader>();
            var fileWriterMock = new Mock<IFileWriter>();

            fileIOMock.SetupGet(x => x.Reader).Returns(fileReaderMock.Object);
            fileIOMock.SetupGet(x => x.Writer).Returns(fileWriterMock.Object);

            return new(fileIOMock, fileReaderMock, fileWriterMock);
        }
    }
    public record FileIOMock(
        Mock<IFileIO> IOMock,
        Mock<IFileReader> ReaderMock,
        Mock<IFileWriter> WriterMock);
}
