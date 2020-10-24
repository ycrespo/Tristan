using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using Tristan.Data.DataAccess;
using Tristan.IoC;
using Tristan.Jobs;
using Tristan.QuartzScheduler;

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
                    ConfigureQuartz(services);
                    
                    services.AddDbContext<TristanContext>(
                        options => options.UseNpgsql(Configuration.GetConnectionString("TristanDb"),
                            npgsqlOptions => npgsqlOptions.UseNodaTime()));

                })
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new TristanModule());
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(Configuration)
                        .CreateLogger();

                    loggingBuilder.AddSerilog(Log.Logger, dispose: true);
                });

        private static void Configure(HostBuilderContext hostContext)
        {
            var env = hostContext.HostingEnvironment.EnvironmentName;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static void ConfigureQuartz(IServiceCollection services)
        {
            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add our job
            services.AddSingleton<LoggerJob>();services.AddSingleton(new JobSchedule(
                jobType: typeof(LoggerJob),
                cronExpression: Configuration.GetValue<string>("Quartz:CronExpression")));
            
            services.AddHostedService<QuartzHostedService>();
        }


    }
}