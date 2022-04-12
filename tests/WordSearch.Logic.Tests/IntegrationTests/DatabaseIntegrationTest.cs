using System.IO;
using Microsoft.Extensions.DependencyInjection;
using WordSearch.Logic.Exceptions.WordsFile;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.Logic.Tests.IntegrationTests
{
    [TestFixture]
    public class DatabaseIntegrationTest
    {
        private const string DatabaseDirectoryPath = "./temp_dbs";
        private const string DatabaseName = "SomeName";
        private const string DatabaseChars = "abc123";
        private const string Word = "aac1";
        private const string WordD1 = "aac12";
        private const string OtherWord = "aboba2";

        private IServiceProvider serviceProvider;
        private IServiceScope serviceScope;
        private IDatabaseManager databaseManager;
        private IDatabase database;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            if (Directory.Exists(DatabaseDirectoryPath))
            {
                Directory.Delete(DatabaseDirectoryPath, true);
            }

            serviceProvider = IntegrationTestHelper.GetServiceProvider(
                x => x.AddWordSearchDatabase(
                    config => config.DatabaseDirectoryPath = DatabaseDirectoryPath));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            IntegrationTestHelper.DisposeServiceProvider(serviceProvider);
        }

        [SetUp]
        public void SetUp()
        {
            serviceScope = serviceProvider.CreateScope();
            var scopeProvider = serviceScope.ServiceProvider;
            databaseManager = scopeProvider.GetRequiredService<IDatabaseManager>();

            databaseManager.Create(DatabaseName, DatabaseChars);
            database = databaseManager.Get(DatabaseName);
        }

        [TearDown]
        public void TearDown()
        {
            serviceScope.Dispose();
            Directory.Delete(DatabaseDirectoryPath, true);
        }

        [Test]
        public void Add_DontThrowException()
        {
            database.Invoking(x => x.Add(Word))
                .Should().NotThrow();
        }

        [Test]
        public void Add_WhenAddOtherWord_DontThrowException()
        {
            database.Add(Word);

            database.Invoking(x => x.Add(OtherWord))
                .Should().NotThrow();
        }

        [Test]
        public void Add_WhenAddSameWord_ThrowWordAlreadyExistsException()
        {
            database.Add(Word);

            database.Invoking(x => x.Add(Word))
               .Should().Throw<WordAlreadyExistsException>();
        }

        [Test]
        public void GetWords_WhenNoWords_ReturnEmptyResult()
        {
            var result = database.GetWords(WordD1, 10);

            result.Should().BeEmpty();
        }

        [Test]
        public void GetWords_WhenSearchedWordExists_ReturnThisWord()
        {
            database.Add(Word);

            var result = database.GetWords(Word, 0);

            result.Should().Contain(Word);
        }

        [Test]
        public void GetWords_WhenNearWordExists_ReturnNearWord()
        {
            var expected = new[] { Word };
            database.Add(Word);

            var result = database.GetWords(WordD1, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetWords_ReturnAllMatchingResults()
        {
            var expected = new[] { Word, WordD1 };
            database.Add(Word);
            database.Add(WordD1);
            database.Add(OtherWord);

            var result = database.GetWords(Word, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Delete_DeletesWord()
        {
            var expected = new[] { WordD1 };
            database.Add(Word);
            database.Add(WordD1);
            database.Add(OtherWord);

            database.Delete(Word);

            var result = database.GetWords(Word, 1);
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Delete_CanDeleteLastWord()
        {
            database.Add(Word);
            database.Add(WordD1);
            database.Delete(WordD1);

            database.Delete(Word);

            var result = database.GetWords(Word, 1);
            result.Should().BeEmpty();
        }

        [Test]
        public void Delete_WhenNoSearchedWord_ThrowWordNotFoundException()
        {
            database.Add(OtherWord);

            database.Invoking(x => x.Delete(Word))
               .Should().Throw<WordNotFoundException>();
        }
    }
}
