using Autofac;
using Tristan.Data.Gateways;

namespace Tristan.IoC
{
    public class TristanModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ContextGateway>().As<IContextGateway>();
        }
    }
}