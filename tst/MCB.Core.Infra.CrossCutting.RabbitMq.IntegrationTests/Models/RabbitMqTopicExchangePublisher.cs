﻿using MCB.Core.Infra.CrossCutting.Abstractions.Serialization;
using MCB.Core.Infra.CrossCutting.RabbitMq.Connection.Interfaces;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using MCB.Core.Infra.CrossCutting.RabbitMq.Publishers;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.IntegrationTests.Models;

public class RabbitMqTopicExchangePublisher
    : RabbitMqExchangePublisherBase
{
    private readonly IProtobufSerializer _protobufSerializer;

    public RabbitMqTopicExchangePublisher(
        IRabbitMqConnection connection,
        RabbitMqExchangeConfig exchangeConfig,
        IProtobufSerializer protobufSerializer
    ) : base(connection, exchangeConfig)
    {
        _protobufSerializer = protobufSerializer;
    }

    protected override IDictionary<string, object>? GetBasicProperties(object subject, Type subjectBaseType)
    {
        return null;
    }
    protected override string GetRoutingKey(object subject, Type subjectBaseType)
    {
        return $"{subjectBaseType.Namespace}*";
    }
    protected override ReadOnlyMemory<byte>? SerializeMessage(object subject, Type subjectBaseType)
    {
        return _protobufSerializer.SerializeToProtobuf(subject);
    }
}