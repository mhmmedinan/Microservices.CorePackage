using Core.Abstractions.Events.External;

namespace Core.Messaging.Transport.RabbitMQ;

public class PublisherChannelFactory : IPublisherChannelFactory
{
    private readonly IPublisherChannelContextPool _contextPool;
    private readonly IQueueReferenceFactory _queueReferenceFactory;

    public PublisherChannelFactory(IPublisherChannelContextPool contextPool,
        IQueueReferenceFactory queueReferenceFactory)
    {
        _contextPool = contextPool ??
                                       throw new ArgumentNullException(nameof(contextPool));
        _queueReferenceFactory =
            queueReferenceFactory ?? throw new ArgumentNullException(nameof(queueReferenceFactory));
    }


    public Task<PublisherChannelContext> CreateAsync(IIntegrationEvent message)
    {
       if(message==null)
            throw new ArgumentNullException(nameof(message));
        var references = _queueReferenceFactory.Create((dynamic)message);
        var result = _contextPool.GetAsync(references);
        return result;
    }
}
