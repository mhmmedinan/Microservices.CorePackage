using Core.Abstractions.Events.External;
using Core.Abstractions.Extensions;
using Core.Abstractions.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Core.Messaging.Transport.RabbitMQ;

public class QueueReferenceFactory : IQueueReferenceFactory
{
    private readonly ConcurrentDictionary<Type, QueueReferences> _queueReferencesCache = new();
    private readonly Func<Type, QueueReferences> _defaultCreator;
    private readonly IServiceProvider _sp;

    public QueueReferenceFactory(
        IServiceProvider sp,
        ISystemInfo systemInfo,
        Func<Type, QueueReferences>? defaultCreator = null)
    {
        _sp = sp ?? throw new ArgumentNullException(nameof(sp));
        var systemInfo1 = systemInfo ?? throw new ArgumentNullException(nameof(systemInfo));

        _defaultCreator = defaultCreator ?? ((Func<Type, QueueReferences>)(messageType =>
        {
            var exchangeName = messageType.Name.ToLower();

            var isEvent = messageType.IsEvent();

            var assemblyName = messageType.Assembly.GetName().Name.Replace(".Domain", "");
            if (assemblyName == "Core")
                assemblyName = systemInfo1.ClientGroup;

            var queueName = isEvent ?
                $"{exchangeName}.{assemblyName}.workers" :
                $"{exchangeName}.workers";

            var dlExchangeName = exchangeName + ".dead";

            var dlQueueName = isEvent ?
                $"{dlExchangeName}.{assemblyName}.workers" :
                $"{dlExchangeName}.workers";

            // if it's an Event, we use the exchange name as routing key,
            // this way all the bond queues will receive it.
            // otherwise we are expecting a single queue to be connected
            // to the exchange, so we use the queue name to prevent duplicate handling
            var routingKey = isEvent ? exchangeName : queueName;

            return new QueueReferences(exchangeName, queueName, routingKey, dlExchangeName, dlQueueName);
        }));
    }

    public QueueReferences Create<TM>(TM message = default)
           where TM : IIntegrationEvent
           => _queueReferencesCache.GetOrAdd(typeof(TM), k => CreateCore<TM>());


    private QueueReferences CreateCore<TM>()
           where TM : IIntegrationEvent
    {
        var creator = _sp.GetService<QueueReferencesPolicy<TM>>();
        return (creator is null) ? _defaultCreator(typeof(TM)) : creator();
    }


 
}

public delegate QueueReferences QueueReferencesPolicy<TM>()
     where TM : IIntegrationEvent;


