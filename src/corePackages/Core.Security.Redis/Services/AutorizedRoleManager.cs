
using StackExchange.Redis;
using System.Text.Json;

namespace Core.Security.Redis.Services;

public class AutorizedRoleManager : IAuthorizedRoleService
{
    private readonly IDatabase _redisDb;
    private const string KeyPrefix = "user_roles:";

    public AutorizedRoleManager(IConnectionMultiplexer redisDb)
    {
        _redisDb = redisDb.GetDatabase();
    }

    public async Task<List<string>?> GetRolesAsync(string username)
    {
        string redisValue = await _redisDb.StringGetAsync(GetKey(username));
        return string.IsNullOrEmpty(redisValue)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(redisValue);
    }

    public async Task AddRolesAsync(string username, IEnumerable<string> roles)
    {
        string serialized = JsonSerializer.Serialize(roles);
        await _redisDb.StringSetAsync(GetKey(username), serialized);
    }

    public async Task RemoveRolesAsync(string username)
    {
        await _redisDb.KeyDeleteAsync(GetKey(username));
    }

    public async Task ClearAsync()
    {
        // Not optimal: depends on your Redis setup (key pattern scanning might be disabled)
        var endpoints = _redisDb.Multiplexer.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _redisDb.Multiplexer.GetServer(endpoint);
            foreach (var key in server.Keys(pattern: $"{KeyPrefix}*"))
                await _redisDb.KeyDeleteAsync(key);
        }
    }

    private static string GetKey(string username) => $"{KeyPrefix}{username}";
}
