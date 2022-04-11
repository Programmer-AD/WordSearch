using System.IO;
using Microsoft.Extensions.DependencyInjection;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.Logic.Tests.IntegrationTests
{
    [TestFixture]
    public class DatabaseManagerIntegrationTest
    {
        private const string DatabaseDirectoryPath = "./temp_dbs";
        private const string DatabaseName = "SomeName";
        private const string OtherDatabaseName = "OtherName";
        private const string DatabaseChars = "abc123";

        private IServiceProvider serviceProvider;
        private IServiceScope serviceScope;
        private IDatabaseManager databaseManager;

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
        }

        [TearDown]
        public void TearDown()
        {
            serviceScope.Dispose();
            Directory.Delete(DatabaseDirectoryPath, true);
        }

        [Test]
        public async Task CreateAsync_MakeFiles()
        {
            await databaseManager.Create(DatabaseName, DatabaseChars);

            Directory.GetFiles(DatabaseDirectoryPath)
                .Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task DeleteAsync_DeleteFiles()
        {
            await databaseManager.Create(DatabaseName, DatabaseChars);

            await databaseManager.Delete(DatabaseName);

            Directory.GetFiles(DatabaseDirectoryPath)
                .Should().BeEmpty();
        }

        [Test]
        public async Task DeleteAsync_MakeDbNotExists()
        {
            await databaseManager.Create(DatabaseName, DatabaseChars);

            await databaseManager.Delete(DatabaseName);

            var exists = await databaseManager.Exists(DatabaseName);
            exists.Should().BeFalse();
        }

        [Test]
        public async Task GetAsync_ReturnsDatabase()
        {
            await databaseManager.Create(DatabaseName, DatabaseChars);

            var database = await databaseManager.Get(DatabaseName);

            database.Should().NotBeNull();
        }

        [Test]
        public async Task ExistsAsync_WhenDbExists_ReturnsTrue()
        {
            await databaseManager.Create(DatabaseName, DatabaseChars);

            var result = await databaseManager.Exists(DatabaseName);

            result.Should().BeTrue();
        }

        [Test]
        public async Task ExistsAsync_WhenDbsNotExists_ReturnsFalse()
        {
            var result = await databaseManager.Exists(DatabaseName);

            result.Should().BeFalse();
        }

        [Test]
        public async Task ExistsAsync_WhenRequestedDbNotExists_ReturnsFalse()
        {
            await databaseManager.Create(OtherDatabaseName, DatabaseChars);

            var result = await databaseManager.Exists(DatabaseName);

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetDbNamesAsync_WhenNoDbs_ReturnsEmptyResult()
        {
            var result = await databaseManager.GetDbNames();

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetDbNamesAsync_FiltersOtherFiles()
        {
            var expected = new[] { DatabaseName };
            await databaseManager.Create(DatabaseName, DatabaseChars);
            File.Create(Path.Combine(DatabaseDirectoryPath, "shit.txt")).Dispose();

            var result = await databaseManager.GetDbNames();

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetDbNamesAsync_ReturnsDbNames()
        {
            var expected = new[] { DatabaseName, OtherDatabaseName };
            await databaseManager.Create(DatabaseName, DatabaseChars);
            await databaseManager.Create(OtherDatabaseName, DatabaseChars);

            var result = await databaseManager.GetDbNames();

            result.Should().BeEquivalentTo(expected);
        }
    }
}
