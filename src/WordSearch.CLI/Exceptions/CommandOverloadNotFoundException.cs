namespace WordSearch.CLI.Exceptions
{
    public class CommandOverloadNotFoundException : Exception
    {
        public CommandOverloadNotFoundException(string commandName)
            : base(GetMessage(commandName))
        {
        }

        private static string GetMessage(string commandName)
        {
            return $"Not found matching overload for command \"{commandName}\"";
        }
    }
}
