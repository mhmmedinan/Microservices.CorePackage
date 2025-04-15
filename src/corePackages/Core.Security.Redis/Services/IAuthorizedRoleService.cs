namespace Core.Security.Redis.Services;

public interface IAuthorizedRoleService
{
    Task<List<string>> GetRolesAsync(string username);
    Task AddRolesAsync(string username, IEnumerable<string> roles);
    Task RemoveRolesAsync(string username);
    Task ClearAsync();
}