using StackExchange.Redis;
using System.Text.Json;

namespace Core.Security.Redis.Session;

public class RedisUserSessionService : IUserSessionService
{
    private readonly IDatabase _redisDb;
    private const string SessionPrefix = "user:sessions:";
    public const int MaxSessionLimit = 3;

    public RedisUserSessionService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task AddSessionAsync(string userId, SessionInfo session)
    {
        var key = SessionPrefix + userId;
        var existing = await GetSessionsAsync(userId);
        existing.Add(session);
        var json = JsonSerializer.Serialize(existing);
        await _redisDb.StringSetAsync(key, json, TimeSpan.FromHours(2));
    }

    public async Task<List<SessionInfo>> GetSessionsAsync(string userId)
    {
        var key = SessionPrefix + userId;
        var value = await _redisDb.StringGetAsync(key);
        if (value.IsNullOrEmpty) return new List<SessionInfo>();
        return JsonSerializer.Deserialize<List<SessionInfo>>(value!) ?? new();
    }

    public async Task RemoveSessionAsync(string userId, string sessionId)
    {
        var key = SessionPrefix + userId;
        var sessions = await GetSessionsAsync(userId);
        var updated = sessions.Where(s => s.SessionId != sessionId).ToList();
        var json = JsonSerializer.Serialize(updated);
        await _redisDb.StringSetAsync(key, json, TimeSpan.FromHours(2));
    }

    public async Task RemoveAllSessionsAsync(string userId)
    {
        var key = SessionPrefix + userId;
        await _redisDb.KeyDeleteAsync(key);
    }

    public async Task<bool> IsSessionLimitExceededAsync(string userId)
    {
        var sessions = await GetSessionsAsync(userId);
        return sessions.Count >= MaxSessionLimit;
    }

    public async Task<bool> ValidateSessionAsync(string userId, string sessionId)
    {
        var sessions = await GetSessionsAsync(userId);
        return sessions.Any(s => s.SessionId == sessionId);
    }
}