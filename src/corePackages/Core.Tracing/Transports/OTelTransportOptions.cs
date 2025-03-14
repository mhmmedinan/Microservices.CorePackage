namespace Core.Tracing.Transports;

public class OTelTransportOptions
{
    public const string InMemoryProducerActivityName = "Diagnostics.InMemoryOutboundMessage";
    public const string InMemoryConsumerActivityName = "Diagnostics.InMemoryInboundMessage";

    public static class Events
    {
        public const string AfterProcessInMemoryMessage = InMemoryConsumerActivityName + ".Stop";
        public const string BeforeProcessInMemoryMessage = InMemoryConsumerActivityName + ".Start";
        public const string BeforeSendInMemoryMessage = InMemoryProducerActivityName + ".Start";
        public const string AfterSendInMemoryMessage = InMemoryProducerActivityName + ".Stop";
        public const string NoSubscriberToPublish = InMemoryProducerActivityName + ".NoSubscriberToPublish";
    }
}
