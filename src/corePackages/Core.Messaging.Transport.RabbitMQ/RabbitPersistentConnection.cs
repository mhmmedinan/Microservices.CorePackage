using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;

namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Implements a persistent connection to RabbitMQ with automatic reconnection capabilities.
/// </summary>
public sealed class RabbitPersistentConnection : IDisposable, IBusConnection
{
    private readonly ILogger<RabbitPersistentConnection> _logger;
    private readonly IConnectionFactory _connectionFactory;
    private IConnection _connection;
    private bool _disposed;

    private readonly object semaphore = new();

    /// <summary>
    /// Initializes a new instance of the RabbitPersistentConnection class.
    /// </summary>
    /// <param name="connectionFactory">The RabbitMQ connection factory.</param>
    /// <param name="logger">Logger for connection-related events.</param>
    /// <exception cref="ArgumentNullException">Thrown when connectionFactory or logger is null.</exception>
    public RabbitPersistentConnection(IConnectionFactory connectionFactory, ILogger<RabbitPersistentConnection> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a value indicating whether the connection is currently established and open.
    /// </summary>
    public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

    /// <summary>
    /// Disposes the connection and releases all resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _connection?.Dispose();
    }

    /// <summary>
    /// Attempts to establish a connection to RabbitMQ with retry policy.
    /// </summary>
    private void TryConnect()
    {
        lock (semaphore)
        {
            if (IsConnected)
                return;

            var policy = Policy
                .Handle<System.Exception>()
                .WaitAndRetry(5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, timeSpan, context) =>
                    {
                        _logger.LogError(ex,
                            $"an exception has occurred while opening RabbitMQ connection: {ex.Message}");
                    });

            _connection = policy.Execute(_connectionFactory.CreateConnection);

            _connection.ConnectionShutdown += (s, e) => TryConnect();
            _connection.CallbackException += (s, e) => TryConnect();
            _connection.ConnectionBlocked += (s, e) => TryConnect();
        }
    }

    /// <summary>
    /// Creates a new channel for communication with RabbitMQ.
    /// </summary>
    /// <returns>A new channel instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no RabbitMQ connections are available.</exception>
    public IModel CreateChannel()
    {
        TryConnect();

        if (!IsConnected)
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        return _connection.CreateModel();
    }
}
