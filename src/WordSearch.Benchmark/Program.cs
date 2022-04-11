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

            await BenchmarkMultiTimes(
                async i => await database.Add(i.ToString()),
                (count, time) => Console.WriteLine($"AddAsync\t{count}\t{time}"));

            await BenchmarkMultiTimes(
                async i => await database.GetWords(i.ToString(), 1),
                (count, time) => Console.WriteLine($"GetWordsAsync\t{count}\t{time}"));

            await BenchmarkMultiTimes(
                async i => await database.Delete((MaxOperationCount - 1 - i).ToString()),
                (count, time) => Console.WriteLine($"DeleteAsync\t{count}\t{time}"));

            Console.WriteLine("Benchmark finish");
            Directory.Delete(DirectoryName, true);
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

            await dbManager.Create(DbName, Chars);
            var database = await dbManager.Get(DbName);
            return database;
        }

        private static async Task<TimeSpan> Benchmark(Func<Task> func)
        {
            var startTime = DateTime.Now;
            await func();
            var endTime = DateTime.Now;
            return endTime - startTime;
        }

        private static async Task BenchmarkMultiTimes(Func<int, Task> func, Action<int, TimeSpan> callback)
        {
            int i = 0;
            var previousTime = TimeSpan.Zero;
            for (int count = 10; count <= MaxOperationCount; count *= 10)
            {
                var addTime = await Benchmark(async () =>
                {
                    for (; i < count; i++)
                    {
                        await func(i);
                    }
                });
                previousTime += addTime;
                callback(count, previousTime);
            }
        }
    }
}
