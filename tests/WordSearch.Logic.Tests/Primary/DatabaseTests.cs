using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Primary;

namespace WordSearch.Logic.Tests.Primary
{
    [TestFixture]
    public class DatabaseTests
    {
        private const string DatabaseName = "SomeName";

        private FileIOMock charFileMocks;
        private FileIOMock wordsFileMocks;
        private Database database;

        [SetUp]
        public void SetUp()
        {
            charFileMocks = GlobalHelpers.MockFileIO();
            wordsFileMocks = GlobalHelpers.MockFileIO();
            //database = new(DatabaseName);
        }
    }
}
