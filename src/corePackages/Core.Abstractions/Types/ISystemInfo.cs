namespace Core.Abstractions.Types
{
    public interface ISystemInfo
    {
        string ClientGroup { get; }
        Guid ClientId { get; }
        bool PublishOnly { get; }
    }
}
