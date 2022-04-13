namespace WordSearch.CLI.Exceptions
{
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException(string commandName) : base(GetMessage(commandName))
        {
        }

        private static string GetMessage(string commandName)
        {
            return $"Not found command \"{commandName}\"";
        }
    }
}
