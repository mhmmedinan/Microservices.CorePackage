using Core.Mailing;
using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Diagnostics;

namespace Core.Application.Pipelines.Performance;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IIntervalRequest
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _stopwatch;
    private readonly IMailService _mailService;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, Stopwatch stopwatch, IMailService mailService)
    {
        _logger = logger;
        _stopwatch = stopwatch;
        _mailService = mailService;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = request.GetType().Name;

        TResponse response;

        try
        {
            _stopwatch.Start();
            response = await next();
        }
        finally
        {
            if (_stopwatch.Elapsed.TotalSeconds > request.Interval)
            {
                string message = $"Performance -> {requestName} {_stopwatch.Elapsed.TotalSeconds.ToString()} s";

                Debug.WriteLine(message);
                _logger.LogInformation(message);
                _mailService.SendEmailAsync(new Mail
                {
                    Subject = "Performans Mail",
                    TextBody = $"{message}",
                    ToList = new List<MailboxAddress> { new(name:"Abb Garjan Group",address:"mail@abbgarjangroup.com")}
                });
            }

            _stopwatch.Restart();
        }

        return response;
    }
}
