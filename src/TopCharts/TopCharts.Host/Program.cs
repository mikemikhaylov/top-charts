using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopCharts.DataAccess.Abstractions;
using TopCharts.DataAccess.Api;
using TopCharts.Domain.Model.Api;

namespace TopCharts.Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var requester = services
                .BuildServiceProvider()
                .GetService<IApiRequester>();
            if (requester!= null)
            {
                var result = await requester.GetTimelineAsync(new TimelineRequest(), CancellationToken.None);
                ;
            }
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env}.json", true, true);

            var config = builder.Build();
            
            services.AddHttpClient<ApiRequester>();
            services.AddSingleton(config.GetSection("VcApi").Get<ApiRequesterOptions>());
            services
                .AddSingleton<IApiRequester, ApiRequester>();
        }
    }
}