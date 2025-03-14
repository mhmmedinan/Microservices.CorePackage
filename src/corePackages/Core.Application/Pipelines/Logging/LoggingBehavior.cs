using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Logging.SeriLog;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Core.Application.Pipelines.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly LoggerServiceBase _loggerServiceBase;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public LoggingBehavior(LoggerServiceBase loggerServiceBase, IHttpContextAccessor httpContextAccessor)
    {
        _loggerServiceBase = loggerServiceBase;
        _httpContextAccessor = httpContextAccessor;
    }


    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        var loggableRequest = request as ILoggableLoginRequest;
        if (loggableRequest != null)
        {
            var password = loggableRequest.Password;
            loggableRequest.Password = null; 
            
            List<LogParameter> logParameters =
            new()
            {
                new LogParameter { Type = request.GetType().Name, Value = request }
            };

            LogDetail logDetail =
                new()
                {
                    MethodName = next.Method.Name,
                    Parameters = logParameters,
                    User = _httpContextAccessor.HttpContext.User.Identity?.Name ?? "?"
                };


            _loggerServiceBase.Info(JsonSerializer.Serialize(logDetail));

            loggableRequest.Password = password;
        }
        else
        {
            var loggableRequest1 = request as ILoggableRequest;
            if (loggableRequest1 != null)
            {
                
                
                List<LogParameter> logParameters =
                new()
                {
                   new LogParameter { Type = request.GetType().Name, Value = request }
                };

                LogDetail logDetail =
                    new()
                    {
                        MethodName = next.Method.Name,
                        Parameters = logParameters,
                        User = _httpContextAccessor.HttpContext.User.Identity?.Name ?? "?"
                    };

                _loggerServiceBase.Info(JsonSerializer.Serialize(logDetail));
            }


        }
        return next();
    }
}