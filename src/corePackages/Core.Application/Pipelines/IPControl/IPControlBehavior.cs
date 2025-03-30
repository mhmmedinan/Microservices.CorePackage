using Core.CrossCuttingConcerns.Exceptions.Types;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core.Application.Pipelines.IPControl;

public class IPControlBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IIPControlRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public IPControlBehavior(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var settings = _configuration.GetSection("IPControlSettings").Get<IPControlSettings>() ?? throw new Exception("IP control settings not found");
        var clientIP = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(clientIP))
            throw new BusinessException("Client IP address could not be determined");

        bool isWhitelisted = settings.WhiteList?.Contains(clientIP) ?? false;
        bool isBlacklisted = settings.BlackList?.Contains(clientIP) ?? false;

        if (isBlacklisted || (!isWhitelisted))
            throw new BusinessException($"Access denied for IP: {clientIP}");

        var response = await next();
        return response;
    }

   
}