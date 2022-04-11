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
        public void Create_MakeFiles()
        {
            databaseManager.Create(DatabaseName, DatabaseChars);

            Directory.GetFiles(DatabaseDirectoryPath)
                .Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Delete_DeleteFiles()
        {
            databaseManager.Create(DatabaseName, DatabaseChars);

            databaseManager.Delete(DatabaseName);

            Directory.GetFiles(DatabaseDirectoryPath)
                .Should().BeEmpty();
        }

        [Test]
        public void Delete_MakeDbNotExists()
        {
            databaseManager.Create(DatabaseName, DatabaseChars);

            databaseManager.Delete(DatabaseName);

            var exists = databaseManager.Exists(DatabaseName);
            exists.Should().BeFalse();
        }

        [Test]
        public void Get_ReturnsDatabase()
        {
            databaseManager.Create(DatabaseName, DatabaseChars);

            var database = databaseManager.Get(DatabaseName);

            database.Should().NotBeNull();
        }

        [Test]
        public void Exists_WhenDbExists_ReturnsTrue()
        {
            databaseManager.Create(DatabaseName, DatabaseChars);

            var result = databaseManager.Exists(DatabaseName);

            result.Should().BeTrue();
        }

        [Test]
        public void Exists_WhenDbsNotExists_ReturnsFalse()
        {
            var result = databaseManager.Exists(DatabaseName);

            result.Should().BeFalse();
        }

        [Test]
        public void Exists_WhenRequestedDbNotExists_ReturnsFalse()
        {
            databaseManager.Create(OtherDatabaseName, DatabaseChars);

            var result = databaseManager.Exists(DatabaseName);

            result.Should().BeFalse();
        }

        [Test]
        public void GetDbNames_WhenNoDbs_ReturnsEmptyResult()
        {
            var result = databaseManager.GetDbNames();

            result.Should().BeEmpty();
        }

        [Test]
        public void GetDbNames_FiltersOtherFiles()
        {
            var expected = new[] { DatabaseName };
            databaseManager.Create(DatabaseName, DatabaseChars);
            File.Create(Path.Combine(DatabaseDirectoryPath, "shit.txt")).Dispose();

            var result = databaseManager.GetDbNames();

            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetDbNames_ReturnsDbNames()
        {
            var expected = new[] { DatabaseName, OtherDatabaseName };
            databaseManager.Create(DatabaseName, DatabaseChars);
            databaseManager.Create(OtherDatabaseName, DatabaseChars);

            var result = databaseManager.GetDbNames();

            result.Should().BeEquivalentTo(expected);
        }
    }
}
