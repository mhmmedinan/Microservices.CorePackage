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
        bool isInAllowedRange = IsIPInRange(clientIP, settings.AllowedRanges);

        if (isBlacklisted || (!isWhitelisted && !isInAllowedRange))
            throw new BusinessException($"Access denied for IP: {clientIP}");

        var response = await next();
        return response;
    }

    private bool IsIPInRange(string ip, List<string>? allowedRanges)
    {
        if (allowedRanges == null || !allowedRanges.Any())
            return false;

        var ipParts = ip.Split('.').Select(int.Parse).ToArray();

        foreach (var range in allowedRanges)
        {
            var rangeParts = range.Split('/');
            var networkAddress = rangeParts[0].Split('.').Select(int.Parse).ToArray();
            var subnetMask = rangeParts.Length > 1 ? int.Parse(rangeParts[1]) : 32;

            bool match = true;
            for (int i = 0; i < 4 && match; i++)
            {
                int mask = subnetMask >= 8 ? 255 : (subnetMask <= 0 ? 0 : (255 << (8 - subnetMask)));
                match = (ipParts[i] & mask) == (networkAddress[i] & mask);
                subnetMask -= 8;
            }

            if (match)
                return true;
        }

        return false;
    }
}