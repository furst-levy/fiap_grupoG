using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Fiap.GrupoG
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
                    var awsOptions = hostContext.Configuration.GetAWSOptions();
                    awsOptions.Credentials = new BasicAWSCredentials(Configuration["AWS:Access_Key"],
                        Configuration["AWS:Secret_Access_Key"]);

                    services.AddSingleton(Configuration);
                    services.AddSingleton(awsOptions.Credentials);
                    services.AddDefaultAWSOptions(awsOptions);
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
