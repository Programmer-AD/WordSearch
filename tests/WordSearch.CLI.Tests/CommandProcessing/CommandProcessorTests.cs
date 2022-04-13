using WordSearch.CLI.CommandProcessing;
using WordSearch.CLI.Exceptions;

namespace WordSearch.CLI.Tests.CommandProcessing
{
    [TestFixture]
    public class CommandProcessorTests
    {
        private CommandProcessor commandProcessor;
        private SampleCommandContainer sampleContainer;

        [SetUp]
        public void SetUp()
        {
            sampleContainer = new SampleCommandContainer();
            commandProcessor = new(sampleContainer);
        }

        [Test]
        public void Process_InvokesNoParamMethods()
        {
            const string command = nameof(SampleCommandContainer.SayHello);

            var result = commandProcessor.Process(command);

            result.Should().Be(SampleCommandContainer.Hello);
        }

        [TestCase("sayhello")]
        [TestCase("SAYHELLO")]
        public void Process_IsCaseInsensitive(string command)
        {
            var result = commandProcessor.Process(command);

            result.Should().Be(SampleCommandContainer.Hello);
        }

        [Test]
        public void Process_InvokesMethodWithParams()
        {
            const int param1 = 10;
            const int param2 = 1;
            const string expected = "9";
            string command = $"{nameof(SampleCommandContainer.Sub)} {param1} {param2}";

            var result = commandProcessor.Process(command);

            result.Should().Be(expected);
        }

        [Test]
        public void Process_WhenMethodWithDefaultParams_InvokesMethodWithoutParams()
        {
            string expected =
                (SampleCommandContainer.ADefaultValue + SampleCommandContainer.BDefaultValue).ToString();
            string command = $"{nameof(SampleCommandContainer.AddDefault)}";

            var result = commandProcessor.Process(command);

            result.Should().Be(expected);
        }

        [Test]
        public void Process_WhenMethodWithDefaultParams_InvokesMethodWithFirstParams()
        {
            const int param1 = 66;
            string expected =
                (param1 + SampleCommandContainer.BDefaultValue).ToString();
            string command = $"{nameof(SampleCommandContainer.AddDefault)} {param1}";

            var result = commandProcessor.Process(command);

            result.Should().Be(expected);
        }

        [Test]
        public void Process_WhenMethodWithDefaultParams_InvokesMethodWithAllParams()
        {
            const int param1 = 66;
            const int param2 = 10;
            string expected = (param1 + param2).ToString();
            string command = $"{nameof(SampleCommandContainer.AddDefault)} {param1} {param2}";

            var result = commandProcessor.Process(command);

            result.Should().Be(expected);
        }

        [Test]
        public void Process_WhenMethodIsNotStatic_InvokesMethod()
        {
            const int param1 = 66;
            string command = $"{nameof(SampleCommandContainer.UpdateField)} {param1}";

            var result = commandProcessor.Process(command);

            sampleContainer.Field.Should().Be(param1);
        }

        [Test]
        public void Process_WhenNoRequestedMethod_ThrowCommandNotFoundException()
        {
            const string command = "unknown";

            commandProcessor.Invoking(x => x.Process(command))
                .Should().Throw<CommandNotFoundException>();
        }

        [Test]
        public void Process_WhenNotEnoughParams_ThrowCommandOverloadNotFoundException()
        {
            string command = $"{nameof(SampleCommandContainer.Sub)}";

            commandProcessor.Invoking(x => x.Process(command))
                .Should().Throw<CommandOverloadNotFoundException>();
        }

        [Test]
        public void Process_WhenTooManyParams_ThrowCommandOverloadNotFoundException()
        {
            const int param1 = 1;
            const int param2 = 2;
            const int param3 = 3;
            string command = $"{nameof(SampleCommandContainer.Sub)} {param1} {param2} {param3}";

            commandProcessor.Invoking(x => x.Process(command))
                .Should().Throw<CommandOverloadNotFoundException>();
        }

        [Test]
        public void Process_DontFindMethodToString()
        {
            string command = $"{nameof(SampleCommandContainer.ToString)}";

            commandProcessor.Invoking(x => x.Process(command))
                .Should().Throw<CommandNotFoundException>();
        }

        [Test]
        public void Process_DontFindMethodsWhereReturnTypeNotString()
        {
            string command = $"{nameof(SampleCommandContainer.Trash)}";

            commandProcessor.Invoking(x => x.Process(command))
                .Should().Throw<CommandNotFoundException>();
        }

    }

    class SampleCommandContainer
    {
        public const string Hello = "Hello";
        public const int BDefaultValue = 5;
        public const int ADefaultValue = 10;

        public int Field;

        public static string SayHello()
        {
            return Hello;
        }

        public static string Sub(int a, int b)
        {
            var c = a - b;
            return c.ToString();
        }

        public static string AddDefault(int a = ADefaultValue, int b = BDefaultValue)
        {
            var c = a + b;
            return c.ToString();
        }

        public string UpdateField(int newValue)
        {
            Field = newValue;
            return Field.ToString();
        }

        public static int Trash()
        {
            return 0;
        }
    }

}
