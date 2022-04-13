using WordSearch.CLI.CommandProcessing;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.CLI.Tests.CommandProcessing
{
    [TestFixture]
    public class CommandProcessorTests
    {
        private Mock<IDatabaseManager> databaseManagerMock;
        private CommandProcessor commandProcessor;

        [SetUp]
        public void SetUp()
        {
            databaseManagerMock = new();
            commandProcessor = new(databaseManagerMock.Object);
        }
    }
}
