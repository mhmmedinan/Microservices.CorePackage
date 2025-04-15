using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Security.Constants;
using Core.Security.Redis.Services;
using Core.Security.Redis.Session;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Core.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizedRoleService _authorizedRoleService;
    private readonly IUserSessionService _userSessionService;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor, IAuthorizedRoleService authorizedRoleService, IUserSessionService userSessionService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizedRoleService = authorizedRoleService;
        _userSessionService = userSessionService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        var sessionId = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        var activeSessions = await _userSessionService.GetSessionsAsync(username);
        bool isActiveSession = activeSessions.Any(s => s.SessionId == sessionId);

        if (!isActiveSession)
            throw new AuthorizationException("Your session is no longer active.");
        var userRoles = await _authorizedRoleService.GetRolesAsync(username);
        if (userRoles == null || !userRoles.Any())
            throw new AuthorizationException("You are not authenticated");
        bool isNotMatchedAUserRoleClaimWithRequestRoles = userRoles.Contains(GeneralOperationClaims.Admin)
                            || request.Roles.Any(role => userRoles.Contains(role));
        if (!isNotMatchedAUserRoleClaimWithRequestRoles)
            throw new AuthorizationException("You are not authorizated");
        TResponse response = await next();
        return response;
    }
}
