using Microsoft.Extensions.DependencyInjection;
using System.IO;
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
        public async Task SetUp()
        {
            serviceScope = serviceProvider.CreateScope();
            var scopeProvider = serviceScope.ServiceProvider;
            databaseManager = scopeProvider.GetRequiredService<IDatabaseManager>();

            await databaseManager.CreateAsync(DatabaseName, DatabaseChars);
            database = await databaseManager.GetAsync(DatabaseName);
        }

        [TearDown]
        public void TearDown()
        {
            serviceScope.Dispose();
            Directory.Delete(DatabaseDirectoryPath, true);
        }

        [Test]
        public async Task AddAsync_DontThrowException()
        {
            await database.Invoking(x => x.AddAsync(Word))
                .Should().NotThrowAsync();
        }

        [Test]
        public async Task AddAsync_WhenAddOtherWord_DontThrowException()
        {
            await database.AddAsync(Word);

            await database.Invoking(x => x.AddAsync(OtherWord))
                .Should().NotThrowAsync();
        }

        [Test]
        public async Task AddAsync_WhenAddSameWord_ThrowWordAlreadyExistsException()
        {
            await database.AddAsync(Word);

            await database.Invoking(x => x.AddAsync(Word))
                .Should().ThrowAsync<WordAlreadyExistsException>();
        }

        [Test]
        public async Task GetWordsAsync_WhenNoWords_ReturnEmptyResult()
        {
            var result = await database.GetWordsAsync(WordD1, 10);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetWordsAsync_WhenSearchedWordExists_ReturnThisWord()
        {
            await database.AddAsync(Word);

            var result = await database.GetWordsAsync(Word, 0);

            result.Should().Contain(Word);
        }

        [Test]
        public async Task GetWordsAsync_WhenNearWordExists_ReturnNearWord()
        {
            var expected = new[] { Word };
            await database.AddAsync(Word);

            var result = await database.GetWordsAsync(WordD1, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetWordsAsync_ReturnAllMatchingResults()
        {
            var expected = new[] { Word, WordD1 };
            await database.AddAsync(Word);
            await database.AddAsync(WordD1);
            await database.AddAsync(OtherWord);

            var result = await database.GetWordsAsync(Word, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task DeleteAsync_DeletesWord()
        {
            var expected = new[] { WordD1 };
            await database.AddAsync(Word);
            await database.AddAsync(WordD1);
            await database.AddAsync(OtherWord);

            await database.DeleteAsync(Word);

            var result = await database.GetWordsAsync(Word, 1);
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task DeleteAsync_WhenNoSearchedWord_ThrowWordNotFoundException()
        {
            await database.AddAsync(OtherWord);

            await database.Invoking(x => x.DeleteAsync(Word))
                .Should().ThrowAsync<WordNotFoundException>();
        }
    }
}
