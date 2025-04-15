namespace Core.Security.Redis.Session;

public class SessionInfo
{

    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public string Ip { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresDate { get; set; } = DateTime.UtcNow.AddHours(2);

}
