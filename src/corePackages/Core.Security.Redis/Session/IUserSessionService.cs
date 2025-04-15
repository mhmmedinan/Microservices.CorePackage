namespace Core.Security.Redis.Session;

public interface IUserSessionService
{
    Task AddSessionAsync(string userId, SessionInfo session);
    Task<List<SessionInfo>> GetSessionsAsync(string userId);
    Task RemoveSessionAsync(string userId, string sessionId);
    Task RemoveAllSessionsAsync(string userId);
    Task<bool> IsSessionLimitExceededAsync(string userId);
    Task<bool> ValidateSessionAsync(string userId, string sessionId);
}
