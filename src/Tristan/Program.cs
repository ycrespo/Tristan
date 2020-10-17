using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Tristan
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                
                Log.Information("Tristan service is up!!");
                
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Tristan service fail to start correctly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices((hostContext, services) =>
                {
                    Configure(hostContext);
                    services.AddHostedService<Worker>();
                }) 
                .ConfigureLogging(loggingBuilder =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .CreateLogger();
                     
                    loggingBuilder.AddSerilog(Log.Logger, dispose: true);
                });
        static void Configure(HostBuilderContext hostContext)
        {
            var env = hostContext.HostingEnvironment.EnvironmentName;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}