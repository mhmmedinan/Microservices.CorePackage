using Core.Abstractions.Events.External;
using Core.Abstractions.Extensions;
using Core.Abstractions.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Implements factory for creating queue references with caching support.
/// </summary>
public class QueueReferenceFactory : IQueueReferenceFactory
{
    private readonly ConcurrentDictionary<Type, QueueReferences> _queueReferencesCache = new();
    private readonly Func<Type, QueueReferences> _defaultCreator;
    private readonly IServiceProvider _sp;

    /// <summary>
    /// Initializes a new instance of the QueueReferenceFactory class.
    /// </summary>
    /// <param name="sp">The service provider.</param>
    /// <param name="systemInfo">System information for queue naming.</param>
    /// <param name="defaultCreator">Optional custom queue reference creator function.</param>
    /// <exception cref="ArgumentNullException">Thrown when sp or systemInfo is null.</exception>
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

    /// <summary>
    /// Creates queue references for a specific message type with caching.
    /// </summary>
    /// <typeparam name="TM">The type of the integration event.</typeparam>
    /// <param name="message">Optional message instance.</param>
    /// <returns>Queue references for the message type.</returns>
    public QueueReferences Create<TM>(TM message = default)
           where TM : IIntegrationEvent
           => _queueReferencesCache.GetOrAdd(typeof(TM), k => CreateCore<TM>());

    /// <summary>
    /// Creates queue references for a specific message type using custom policy or default creator.
    /// </summary>
    /// <typeparam name="TM">The type of the integration event.</typeparam>
    /// <returns>Queue references for the message type.</returns>
    private QueueReferences CreateCore<TM>()
           where TM : IIntegrationEvent
    {
        var creator = _sp.GetService<QueueReferencesPolicy<TM>>();
        return (creator is null) ? _defaultCreator(typeof(TM)) : creator();
    }
}

/// <summary>
/// Delegate for custom queue reference creation policy.
/// </summary>
/// <typeparam name="TM">The type of the integration event.</typeparam>
/// <returns>Queue references for the message type.</returns>
public delegate QueueReferences QueueReferencesPolicy<TM>()
     where TM : IIntegrationEvent;


