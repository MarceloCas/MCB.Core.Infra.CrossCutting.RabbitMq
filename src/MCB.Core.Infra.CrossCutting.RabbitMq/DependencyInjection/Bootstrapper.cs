using MCB.Core.Infra.CrossCutting.DependencyInjection.Abstractions.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.DependencyInjection;
public static class Bootstrapper
{
    public static void ConfigureDependencyInjection(
        IDependencyInjectionContainer dependencyInjectionContainer,
        Func<RabbitMqConnectionConfig> defaultRabbitMqConnectionConfigAction
    )
    {

    }
}
