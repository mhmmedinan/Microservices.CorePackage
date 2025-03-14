using MediatR;
using System.ComponentModel;

namespace Core.Scheduling.Hangfire;

public class CommandProcessorHangfireBridge
{
    private readonly IMediator _mediator;

    public CommandProcessorHangfireBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    [DisplayName("{1}")]
    public Task Send(IInternalCommand command, string description = "")
    {
        return _mediator.Send(command);
    }

    [DisplayName("{0}")]
    public Task Send(string jobName, IInternalCommand command)
    {
        return _mediator.Send(command);
    }
}
