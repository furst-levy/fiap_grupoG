using Fiap.GrupoG.Jobs.AwsComprehend;
using Fiap.GrupoG.Jobs.Twitter;
using Fiap.GrupoG.Mongo.Interfaces;
using Fiap.GrupoG.Mongo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Fiap.GrupoG.Jobs
{
    public class Program
    {
        static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configApp) => { BuildJsonConfig(configApp); })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(Configuration);
                    services.AddSingleton<ITweetRepository, TweetRepository>();
                    services.AddSingleton<IAwsComprehendServices, AwsComprehendServices>();

                    services.AddHttpClient<ITwitterService, TwitterService>(c =>
                    {
                        c.BaseAddress = new Uri(Configuration.GetSection("Twitter:BaseUrl").Value);
                        c.DefaultRequestHeaders.Add("Accept", "application/json");
                    });

                    services.AddHostedService<Worker>();
                });

        private static void BuildJsonConfig(IConfigurationBuilder configApp)
        {
            configApp.AddJsonFile("appsettings.json", false, true);
            configApp.AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}.json", true);
            configApp.AddEnvironmentVariables();

            Configuration = configApp.Build();
        }
    }
}
