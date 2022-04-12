using Microsoft.Extensions.DependencyInjection;

namespace WordSearch.Logic.Tests.IntegrationTests
{
    internal static class IntegrationTestHelper
    {
        public static IServiceProvider GetServiceProvider(Action<IServiceCollection> setupServices)
        {
            var services = new ServiceCollection();
            setupServices(services);
            return services.BuildServiceProvider();
        }

        public static void DisposeServiceProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
