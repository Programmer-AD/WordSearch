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

        private static void Main()
        {
            if (Directory.Exists(DirectoryName))
            {
                Directory.Delete(DirectoryName, true);
            }

            var database = GetDatabase();

            BenchmarkMultiTimes(
                i => database.Add(i.ToString()),
                (count, time) => Console.WriteLine($"Add\t{count}\t{time}"));

            BenchmarkMultiTimes(
                i => database.GetWords(i.ToString(), 1),
                (count, time) => Console.WriteLine($"GetWords\t{count}\t{time}"));

            BenchmarkMultiTimes(
                i => database.Delete((MaxOperationCount - 1 - i).ToString()),
                (count, time) => Console.WriteLine($"Delete\t{count}\t{time}"));

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

        private static IDatabase GetDatabase()
        {
            var provider = GetServiceProvider();
            var dbManager = provider.GetRequiredService<IDatabaseManager>();

            dbManager.Create(DbName, Chars);
            var database = dbManager.Get(DbName);
            return database;
        }

        private static TimeSpan Benchmark(Action action)
        {
            var startTime = DateTime.Now;
            action();
            var endTime = DateTime.Now;
            var result = endTime - startTime;
            return result;
        }

        private static void BenchmarkMultiTimes(Action<int> action, Action<int, TimeSpan> callback)
        {
            int i = 0;
            var previousTime = TimeSpan.Zero;
            for (int count = 10; count <= MaxOperationCount; count *= 10)
            {
                var addTime = Benchmark(() =>
                {
                    for (; i < count; i++)
                    {
                        action(i);
                    }
                });
                previousTime += addTime;
                callback(count, previousTime);
            }
        }
    }
}
