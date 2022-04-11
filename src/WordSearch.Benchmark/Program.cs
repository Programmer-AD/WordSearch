using Microsoft.Extensions.DependencyInjection;
using WordSearch.Logic;
using WordSearch.Logic.Interfaces.Primary;

namespace WordSearch.Benchmark
{
    internal static class Program
    {
        const string DbName = "SomeName";
        const string Chars = "0123456789";
        const string DirectoryName = "./test_dbs";
        private const int MaxOperationCount = 10000;

        private static async Task Main()
        {
            if (Directory.Exists(DirectoryName))
            {
                Directory.Delete(DirectoryName, true);
            }

            var database = await GetDatabase();

            int i = 0;
            var previousTime = TimeSpan.Zero;
            for (int count = 10; count <= MaxOperationCount; count *= 10)
            {
                var addTime = await Benchmark(async () =>
                {
                    for (; i < count; i++)
                    {
                        await database.AddAsync(i.ToString());
                    }
                });
                previousTime += addTime;
                Console.WriteLine($"AddAsync\t{count}\t{previousTime}");
            }

            i = 0;
            previousTime = TimeSpan.Zero;
            for (int count = 10; count <= MaxOperationCount; count *= 10)
            {
                var addTime = await Benchmark(async () =>
                {
                    for (; i < count; i++)
                    {
                        await database.DeleteAsync((MaxOperationCount - 1 - i).ToString());
                    }
                });
                previousTime += addTime;
                Console.WriteLine($"DeleteAsync\t{count}\t{previousTime}");
            }

            Console.WriteLine("Benchmark finish");
            Directory.Delete(DirectoryName, true);
            Console.ReadLine();
            Console.ReadLine();
        }

        private static ServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddWordSearchDatabase(config => config.DatabaseDirectoryPath = DirectoryName);
            var provider = services.BuildServiceProvider();
            return provider;
        }

        private static async Task<IDatabase> GetDatabase()
        {
            var provider = GetServiceProvider();
            var dbManager = provider.GetRequiredService<IDatabaseManager>();

            await dbManager.CreateAsync(DbName, Chars);
            var database = await dbManager.GetAsync(DbName);
            return database;
        }

        private static async Task<TimeSpan> Benchmark(Func<Task> func)
        {
            var startTime = DateTime.Now;
            await func();
            var endTime = DateTime.Now;
            return endTime - startTime;
        }
    }
}
