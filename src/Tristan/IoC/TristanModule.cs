using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Tristan.Core.Gateways;
using Tristan.Core.Models;
using Tristan.Core.Services;
using Tristan.Core.Validators;
using Tristan.Data.Gateways;
using Tristan.Helpers;

namespace Tristan.IoC
{
    public class TristanModule : Module
    {
        private readonly IConfiguration _configuration;

        public TristanModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FilenameValidator>().As<IValidator<Doc>>();
            builder.RegisterType<ExtensionValidator>().As<IValidator<Doc>>()
                .WithParameter("extensions", _configuration.GetSection("Validation:SupportedExtensions").Get<IEnumerable<string>>());
            builder.RegisterType<NumberOfRetryValidator>().As<IValidator<Doc>>()
                .WithParameter("maxNumberOfRetry", _configuration.GetValue<int>("Validation:MaxNumberOfRetry"));
            builder.RegisterType<DocValidator>().As<IDocValidator>();

            builder.RegisterType<FtpGateway>().As<IFtpGateway>();
            builder.RegisterType<DocManager>().As<IDocManager>();
            builder.RegisterType<SystemDirectoryService>().As<IDirectoryService>();

            builder.RegisterType<ContextGateway>().As<IContextGateway>();
        }
    }
}