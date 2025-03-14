using MediatR;

namespace Core.Abstractions.Events;

public interface IEvent:INotification
{
    public Guid EventId { get; }
    public long EventVersion { get; }
    public DateTime OccurredOn { get; }
    public DateTimeOffset TimeStamp { get; }
    public string EventType { get;}
}
