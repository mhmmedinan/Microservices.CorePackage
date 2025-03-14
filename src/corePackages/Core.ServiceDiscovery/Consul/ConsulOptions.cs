namespace Core.ServiceDiscovery.Consul;

public class ConsulOptions
{
    public string Id { get; set; }

    public string ConsulAddress { get; set; }

    public Uri ServiceAddress { get; set; }

    public string ServiceName { get; set; }

    public bool DisableAgentCheck { get; set; }

    public string[] Tags { get; set; }

    public int? ServiceDeRegistrationSeconds { get; set; }

    public int? IntervalSeconds { get; set; }

    public string HealthCheckEndPoint { get; set; }
}