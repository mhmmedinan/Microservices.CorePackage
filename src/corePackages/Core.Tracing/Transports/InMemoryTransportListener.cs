using Core.Tracing.Messaging.Events;
using Microsoft.Extensions.DiagnosticAdapter;
using System.Diagnostics;

namespace Core.Tracing.Transports;

public class InMemoryTransportListener
{
    public static string InBoundName =>
        OTelTransportOptions.InMemoryConsumerActivityName;

    public static string OutBoundName =>
        OTelTransportOptions.InMemoryProducerActivityName;

    // Published message parameter name and published property name should be identical
    [DiagnosticName(OTelTransportOptions.Events.BeforeProcessInMemoryMessage)]
    public virtual void BeforeProcessInMemoryMessage(BeforeProcessMessage payload)
    {
        Console.WriteLine(
            $"raising BeforeProcessInMemoryMessage event for message with message id: {payload.EventData.EventId} - activity id: '{Activity.Current?.Id}'");
    }

    [DiagnosticName(OTelTransportOptions.Events.AfterProcessInMemoryMessage)]
    public virtual void AfterProcessInMemoryMessage(AfterProcessMessage payload)
    {
        Console.WriteLine(
            $"raising AfterProcessInMemoryMessage event for message with message id: {payload.EventData.EventId} - activity id: '{Activity.Current?.Id}'");
    }


    [DiagnosticName(OTelTransportOptions.Events.AfterSendInMemoryMessage)]
    public virtual void AfterSendInMemoryMessage(AfterSendMessage payload)
    {
        Console.WriteLine(
            $"raising AfterSendInMemoryMessage event for message with message id: {payload.EventData.EventId} - activity id: '{Activity.Current?.Id}'");
    }


    [DiagnosticName(OTelTransportOptions.Events.BeforeSendInMemoryMessage)]
    public virtual void BeforeSendInMemoryMessage(BeforeSendMessage payload)
    {
        Console.WriteLine(
            $"raising BeforeSendInMemoryMessage event for message with message id: {payload.EventData.EventId} - activity id: '{Activity.Current?.Id}'");
    }
}