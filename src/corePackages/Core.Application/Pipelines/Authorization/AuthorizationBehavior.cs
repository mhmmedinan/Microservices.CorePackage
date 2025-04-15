using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Security.Constants;
using Core.Security.Redis.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizedRoleService _authorizedRoleService;

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor, IAuthorizedRoleService authorizedRoleService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizedRoleService = authorizedRoleService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        var username = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        var userRoles = await _authorizedRoleService.GetRolesAsync(username);
        if (userRoles == null || !userRoles.Any())
            throw new AuthorizationException("You are not authenticated");
        bool isNotMatchedAUserRoleClaimWithRequestRoles = userRoles.Contains(GeneralOperationClaims.Admin)
                            || request.Roles.Any(role => userRoles.Contains(role));
        if (isNotMatchedAUserRoleClaimWithRequestRoles)
            throw new AuthorizationException("You are not authorizated");
        TResponse response = await next();
        return response;
    }
}
