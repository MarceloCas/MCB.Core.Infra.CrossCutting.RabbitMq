namespace MCB.Core.Infra.CrossCutting.RabbitMq.Models;

public record class RabbitMqConnectionConfig(
    string ClientProvidedName,
    string HostName,
    int Port,
    string Username,
    string Password,
    string VirtualHost,
    bool DispatchConsumersAsync,
    bool AutoConnect,
    bool AutomaticRecoveryEnabled,
    TimeSpan NetworkRecoveryInterval,
    bool TopologyRecoveryEnabled,
    TimeSpan RequestedHeartbeat
);
