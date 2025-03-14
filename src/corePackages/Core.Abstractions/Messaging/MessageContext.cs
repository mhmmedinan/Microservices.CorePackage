namespace Core.Abstractions.Messaging
{
    public class MessageContext : IMessageContext
    {
        public static MessageContext Default => new();
    }
}
