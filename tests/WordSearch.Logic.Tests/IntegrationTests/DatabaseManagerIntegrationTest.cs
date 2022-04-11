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
            await databaseManager.CreateAsync(DatabaseName, DatabaseChars);

            Directory.GetFiles(DatabaseDirectoryPath)
                .Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task DeleteAsync_DeleteFiles()
        {
            await databaseManager.CreateAsync(DatabaseName, DatabaseChars);

            await databaseManager.DeleteAsync(DatabaseName);

            Directory.GetFiles(DatabaseDirectoryPath)
                .Should().BeEmpty();
        }

        [Test]
        public async Task DeleteAsync_MakeDbNotExists()
        {
            await databaseManager.CreateAsync(DatabaseName, DatabaseChars);

            await databaseManager.DeleteAsync(DatabaseName);

            var exists = await databaseManager.ExistsAsync(DatabaseName);
            exists.Should().BeFalse();
        }

        [Test]
        public async Task ExistsAsync_WhenDbExists_ReturnsTrue()
        {
            await databaseManager.CreateAsync(DatabaseName, DatabaseChars);

            var result = await databaseManager.ExistsAsync(DatabaseName);

            result.Should().BeTrue();
        }

        [Test]
        public async Task ExistsAsync_WhenDbsNotExists_ReturnsFalse()
        {
            var result = await databaseManager.ExistsAsync(DatabaseName);

            result.Should().BeFalse();
        }

        [Test]
        public async Task ExistsAsync_WhenRequestedDbNotExists_ReturnsFalse()
        {
            await databaseManager.CreateAsync(OtherDatabaseName, DatabaseChars);

            var result = await databaseManager.ExistsAsync(DatabaseName);

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetDbNamesAsync_WhenNoDbs_ReturnsEmptyResult()
        {
            var result = await databaseManager.GetDbNamesAsync();

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetDbNamesAsync_FiltersOtherFiles()
        {
            var expected = new[] { DatabaseName };
            await databaseManager.CreateAsync(DatabaseName, DatabaseChars);
            File.Create(Path.Combine(DatabaseDirectoryPath, "shit.txt")).Dispose();

            var result = await databaseManager.GetDbNamesAsync();

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetDbNamesAsync_ReturnsDbNames()
        {
            var expected = new[] { DatabaseName, OtherDatabaseName };
            await databaseManager.CreateAsync(DatabaseName, DatabaseChars);
            await databaseManager.CreateAsync(OtherDatabaseName, DatabaseChars);

            var result = await databaseManager.GetDbNamesAsync();

            result.Should().BeEquivalentTo(expected);
        }
    }
}
