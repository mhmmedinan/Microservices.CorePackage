using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.ServiceDiscovery.Consul;

public class ConsulServiceDiscoveryHostedService : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly ConsulOptions _consulOptions;
    private readonly ILogger<ConsulServiceDiscoveryHostedService> _logger;

    private string _registrationId;

    public ConsulServiceDiscoveryHostedService(
        IConsulClient consulClient,
        IOptions<ConsulOptions> consulOptions,
        ILogger<ConsulServiceDiscoveryHostedService> logger)
    {
        _consulClient = consulClient ?? throw new ArgumentNullException(nameof(consulClient));
        _consulOptions = consulOptions.Value ?? throw new ArgumentNullException(nameof(consulOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    // Registers service to Consul registry
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _registrationId = $"{_consulOptions.ServiceName}";

        var registration = new AgentServiceRegistration
        {
            ID = _registrationId,
            Name = _consulOptions.ServiceName,
            Address = _consulOptions.ServiceAddress.Host,
            Port = _consulOptions.ServiceAddress.Port,
            Tags = _consulOptions.Tags
        };

        if (!_consulOptions.DisableAgentCheck)
        {
            var secondsAfterServiceDeRegistration = _consulOptions.ServiceDeRegistrationSeconds ?? 60;
            var intervalSeconds = _consulOptions.IntervalSeconds ?? 30;

            registration.Check = new AgentServiceCheck
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(secondsAfterServiceDeRegistration),
                Interval = TimeSpan.FromSeconds(intervalSeconds),
                HTTP = string.IsNullOrEmpty(_consulOptions.HealthCheckEndPoint)
                    ? new Uri(_consulOptions.ServiceAddress, "healthchecks").OriginalString
                    : $"http://{_consulOptions.ServiceAddress.Host}:{_consulOptions.ServiceAddress.Port}/api/values/{_consulOptions.HealthCheckEndPoint}",
            };
        }

        _logger.LogInformation("Registering service with registration Id {RegistrationId} with Consul",
            _registrationId);

        // Deregister already registered service
        await _consulClient.Agent.ServiceDeregister(registration.ID, cancellationToken);

        // Registers service
        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
    }


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("De-registering service with registration Id {RegistrationId} from Consul",
            _registrationId);

        return _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
    }
}
