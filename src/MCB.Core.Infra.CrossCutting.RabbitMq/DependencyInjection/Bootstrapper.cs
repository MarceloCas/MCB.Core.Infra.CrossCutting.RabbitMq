using MCB.Core.Infra.CrossCutting.DependencyInjection.Abstractions.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Connection;
using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.DependencyInjection;
public static class Bootstrapper
{
    public static void ConfigureDependencyInjection(
        IDependencyInjectionContainer dependencyInjectionContainer,
        Func<RabbitMqConnectionConfig> rabbitMqConnectionConfigAction
    )
    {
        dependencyInjectionContainer.RegisterSingleton(dependencyInjectionContainer => rabbitMqConnectionConfigAction());
        dependencyInjectionContainer.RegisterSingleton<IRabbitMqConnection, RabbitMqConnection>();
    }
}
