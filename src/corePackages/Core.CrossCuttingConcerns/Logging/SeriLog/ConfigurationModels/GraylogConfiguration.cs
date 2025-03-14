namespace Core.CrossCuttingConcerns.Logging.SeriLog.ConfigurationModels;

public class GraylogConfiguration
{
    public string? HostnameOrAddress { get; set; }
    public int Port { get; set; }
}
