using WordSearch.CLI.CommandProcessing;
using WordSearch.Logic.Exceptions.DatabaseManager;
using WordSearch.Logic.Exceptions.WordsFile;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.CLI.Tests.CommandProcessing
{
    [TestFixture]
    public class DatabaseCommandContainerTests
    {
        private const string DbName = "test_db";
        private const string Chars = "123";
        private const string Word = "121";
        private const byte MaxDifference = 2;


        private Mock<IDatabaseManager> databaseManagerMock;
        private DatabaseCommandContainer commandContainer;

        [SetUp]
        public void SetUp()
        {
            databaseManagerMock = new();
            commandContainer = new(databaseManagerMock.Object);
        }

        [Test]
        public void CreateDB_CallDatabaseManagerCreate()
        {
            commandContainer.CreateDB(DbName, Chars);

            databaseManagerMock.Verify(x => x.Create(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void CreateDB_WhenOccursDatabaseException_DontThrowException()
        {
            databaseManagerMock.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<DatabaseException>();

            commandContainer.Invoking(x => x.CreateDB(DbName, Chars))
                .Should().NotThrow();
        }

        [Test]
        public void DeleteDB_CallDatabaseManagerDelete()
        {
            commandContainer.DeleteDB(DbName);

            databaseManagerMock.Verify(x => x.Delete(It.IsAny<string>()));
        }

        [Test]
        public void DeleteDB_WhenOccursDatabaseException_DontThrowException()
        {
            databaseManagerMock.Setup(x => x.Delete(It.IsAny<string>()))
                .Throws<DatabaseException>();

            commandContainer.Invoking(x => x.DeleteDB(DbName))
                .Should().NotThrow();
        }

        [Test]
        public void Use_CallDatabaseManagerGet()
        {
            commandContainer.Use(DbName);

            databaseManagerMock.Verify(x => x.Get(It.IsAny<string>()));
        }

        [Test]
        public void Use_WhenOccursDatabaseException_DontThrowException()
        {
            databaseManagerMock.Setup(x => x.Get(It.IsAny<string>()))
                .Throws<DatabaseException>();

            commandContainer.Invoking(x => x.Use(DbName))
                .Should().NotThrow();
        }

        [Test]
        public void ShowDbs_CallDatabaseManagerGetDbNames()
        {
            commandContainer.ShowDbs();

            databaseManagerMock.Verify(x => x.GetDbNames());
        }

        [Test]
        public void AddWord_WhenDbNotUsed_DontThrowException()
        {
            commandContainer.Invoking(x => x.AddWord(Word))
                .Should().NotThrow();
        }

        [Test]
        public void AddWord_CallDatabaseAdd()
        {
            var databaseMock = SetDbUsed();

            commandContainer.AddWord(Word);

            databaseMock.Verify(x => x.Add(It.IsAny<string>()));
        }

        [Test]
        public void AddWord_WhenOccursWordException_DontThrowException()
        {
            var databaseMock = SetDbUsed();
            databaseMock.Setup(x => x.Add(It.IsAny<string>()))
                .Throws<WordException>();

            commandContainer.Invoking(x => x.AddWord(Word))
                .Should().NotThrow();
        }
        [Test]
        public void DeleteWord_WhenDbNotUsed_DontThrowException()
        {
            commandContainer.Invoking(x => x.DeleteWord(Word))
                .Should().NotThrow();
        }

        [Test]
        public void DeleteWord_CallDatabaseDelete()
        {
            var databaseMock = SetDbUsed();

            commandContainer.DeleteWord(Word);

            databaseMock.Verify(x => x.Delete(It.IsAny<string>()));
        }

        [Test]
        public void DeleteWord_WhenOccursWordException_DontThrowException()
        {
            var databaseMock = SetDbUsed();
            databaseMock.Setup(x => x.Delete(It.IsAny<string>()))
                .Throws<WordException>();

            commandContainer.Invoking(x => x.DeleteWord(Word))
                .Should().NotThrow();
        }

        [Test]
        public void FindWords_WhenDbNotUsed_DontThrowException()
        {
            commandContainer.Invoking(x => x.FindWords(Word, MaxDifference))
                .Should().NotThrow();
        }

        [Test]
        public void FindWords_CallDatabaseGetWords()
        {
            var databaseMock = SetDbUsed();

            commandContainer.FindWords(Word, MaxDifference);

            databaseMock.Verify(x => x.GetWords(It.IsAny<string>(), It.IsAny<byte>()));
        }

        [Test]
        public void ShowChars_WhenDbNotUsed_DontThrowException()
        {
            commandContainer.Invoking(x => x.ShowChars())
                .Should().NotThrow();
        }

        [Test]
        public void ShowChars_GetsDatabaseChars()
        {
            var databaseMock = SetDbUsed();

            commandContainer.ShowChars();

            databaseMock.VerifyGet(x => x.Chars);
        }

        private Mock<IDatabase> SetDbUsed()
        {
            var databaseMock = new Mock<IDatabase>();
            databaseManagerMock.Setup(x => x.Get(It.IsAny<string>()))
                .Returns(databaseMock.Object);

            commandContainer.Use(DbName);

            return databaseMock;
        }
    }
}
