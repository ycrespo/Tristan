#nullable enable
using System;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Tristan.Jobs;
using Tristan.QuartzScheduler;

namespace Tristan.IoC
{
    public class QuartzModule : Autofac.Module
    {
        private readonly IConfiguration? _configuration;

        public QuartzModule(IConfiguration? configuration)
        {
            _configuration = configuration;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            RegisterQuartz(builder);

            builder.RegisterType<JobOptions>()
                .AsSelf()
                .WithParameters(new[]
                {
                    new NamedParameter("jobType", typeof(MoveFilesJob)),
                    new NamedParameter("cronExpression", _configuration.GetValue<string>("Quartz:CronExpression") 
                                                         ?? throw new NullReferenceException("Cron Expression cannot be null"))
                });
        }

        private static void RegisterQuartz(ContainerBuilder builder)
        {
            builder.RegisterType<StdSchedulerFactory>().As<ISchedulerFactory>().SingleInstance();

            builder.RegisterType<JobFactory>()
                .As<IJobFactory>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(Program))!)
                .AssignableTo<IJob>()
                .AsSelf()
                .SingleInstance();
        }
    }
}