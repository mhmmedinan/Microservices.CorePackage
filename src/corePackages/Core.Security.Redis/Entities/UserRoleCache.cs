namespace Core.Security.Redis.Entities;

public class UserRolesCache
{
    public string Username { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}