using WordSearch.Logic.Interfaces;
using WordSearch.Logic.Interfaces.IO;

namespace WordSearch.Logic.Tests
{
    [TestFixture]
    public class DatabaseManagerTests
    {
        private static readonly DatabaseConfig databaseConfig = new()
        {
            DatabaseDirectory = "./test_dbs"
        };

        private Mock<IFileManager> fileManagerMock;
        private Mock<IFileIOFactory> fileIOFactoryMock;
        private Mock<IDatabaseFactory> databaseFactoryMock;
        private DatabaseManager databaseManager;

        [SetUp]
        public void SetUp()
        {
            fileManagerMock = new();
            fileIOFactoryMock = new();
            databaseFactoryMock = new();

            databaseManager = new(
                fileManagerMock.Object,
                fileIOFactoryMock.Object,
                databaseFactoryMock.Object,
                databaseConfig);
        }
    }
}
