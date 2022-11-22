using MCB.Core.Infra.CrossCutting.DesignPatterns.Abstractions.Observer;
using MCB.Core.Infra.CrossCutting.RabbitMq.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCB.Core.Infra.CrossCutting.RabbitMq.Consumers.Interfaces;
public interface IRabbitMqConsumer
    : ISubscriber<RabbitMqMessageEnvelop>
{
}
