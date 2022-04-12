using Microsoft.Extensions.DependencyInjection;
using WordSearch.Logic;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.CLI
{
    internal static class Program
    {
        private static void Main()
        {
            using var serviceProvider = GetServiceProvider();
            var databaseManager = serviceProvider.GetService<IDatabaseManager>();
            var commandProcessor = new CommandProcessor(databaseManager);

            Console.WriteLine("WordSearch database CLI");
            Console.WriteLine();
            while (true)
            {
                var usedDbName = commandProcessor.UsedDatabase?.Name ?? "(not selected)";
                Console.Write(usedDbName);
                Console.Write(" > ");
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    var resultMessage = commandProcessor.Process(input);
                    Console.WriteLine(resultMessage);
                    Console.WriteLine();
                }
            }
        }

        private static ServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddWordSearchDatabase();
            var provider = services.BuildServiceProvider();
            return provider;
        }
    }
}