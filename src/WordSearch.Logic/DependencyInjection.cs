using Microsoft.Extensions.DependencyInjection;
using WordSearch.Logic.Encoders;
using WordSearch.Logic.Interfaces.Encoders;
using WordSearch.Logic.Interfaces.IO;
using WordSearch.Logic.Interfaces.Primary;
using WordSearch.Logic.IO;
using WordSearch.Logic.Primary;

namespace WordSearch.Logic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWordSearchDatabase(
            this IServiceCollection services,
            Action<DatabaseConfig> configure = null)
        {
            services.AddScoped<IDatabaseManager, DatabaseManager>();

            services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
            services.AddSingleton<IFileIOFactory, FileIOFactory>();
            services.AddSingleton<IFileManager, FileManager>();
            services.AddSingleton<IWordEncoderFactory, WordEncoderFactory>();

            if (configure != null)
            {
                services.Configure(configure);
            }

            return services;
        }
    }
}
