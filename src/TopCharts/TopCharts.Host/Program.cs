using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using TopCharts.DataAccess.Abstractions;
using TopCharts.DataAccess.Api;
using TopCharts.DataAccess.Db;
using TopCharts.Domain.Services;

namespace TopCharts.Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var posting = services
                .BuildServiceProvider()
                .GetService<IPostingService>();
            if (posting!= null)
            {
                await posting.ProcessAsync(CancellationToken.None);
            }
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddUserSecrets(Assembly.GetEntryAssembly());

            var config = builder.Build();
            
            services.AddHttpClient<ApiRequester>().AddTransientHttpErrorPolicy(p => 
                p.WaitAndRetryAsync(6, retry => TimeSpan.FromMilliseconds(10000 * retry)));;
            services.AddSingleton(config.GetSection("VcApi").Get<ApiRequesterOptions>());
            services.AddSingleton(config.GetSection("Db").Get<DbOptions>());
            services
                .AddSingleton<IApiRequester, ApiRequester>();
            services
                .AddSingleton<IKeyValueRepository, KeyValueRepository>();
            services
                .AddSingleton<IItemRepository, ItemRepository>();
            services
                .AddSingleton<IPostingService, PostingService>();
        }
    }
}