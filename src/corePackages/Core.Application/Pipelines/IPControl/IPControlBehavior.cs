using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.CrossCuttingConcerns.Utilities.Helpers.IPControl;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Core.Application.Pipelines.IPControl;

public class IPControlBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>,IIPControlRequest
{
    private readonly IIPControlHelper _ipControlHelper;
    private readonly IHttpContextAccessor _contextAccessor;

    public IPControlBehavior(IIPControlHelper ipControlHelper, IHttpContextAccessor contextAccessor)
    {
        _ipControlHelper = ipControlHelper;
        _contextAccessor = contextAccessor;
    }

    public async Task<TResponse> Handle
        (TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var ipAddress = getIpAddress();
        var ips = _ipControlHelper.GetAllowedIPListAsync();

        if (!IsIPAllowed(ips, ipAddress))
        {
            throw new NotFoundException("no such page found");
        }

        return await next();

    }

    protected string? getIpAddress()
    {
        if (_contextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For")) 
            return _contextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
        return _contextAccessor.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    private bool IsIPAllowed(List<string> allowedIPs, string remoteIp)
    {
        return allowedIPs.Any(ip => IPAddress.Parse(ip).Equals(remoteIp));
    }
}
