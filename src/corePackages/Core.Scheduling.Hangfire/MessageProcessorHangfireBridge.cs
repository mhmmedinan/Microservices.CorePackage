using Core.Abstractions.Messaging;
using MediatR;
using System.ComponentModel;

namespace Core.Scheduling.Hangfire;

public class MessageProcessorHangfireBridge
{
    private readonly IMediator _mediator;

    public MessageProcessorHangfireBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    [DisplayName("{1}")]
    public Task Send(IMessage message, string description = "")
    {
        return _mediator.Send(message);
    }

    [DisplayName("{0}")]
    public Task Send(string jobName,IMessage message)
    {
        return _mediator.Send(message);
    }
}
