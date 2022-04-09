using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.Tests
{
    internal static class GlobalHelpers
    {
        public static (Mock<IFileIO>, Mock<IFileReader>, Mock<IFileWriter>) MockFileIO()
        {
            var fileIOMock = new Mock<IFileIO>();
            var fileReaderMock = new Mock<IFileReader>();
            var fileWriterMock = new Mock<IFileWriter>();

            fileIOMock.SetupGet(x => x.Reader).Returns(fileReaderMock.Object);
            fileIOMock.SetupGet(x => x.Writer).Returns(fileWriterMock.Object);

            return (fileIOMock, fileReaderMock, fileWriterMock);
        }
    }
}
