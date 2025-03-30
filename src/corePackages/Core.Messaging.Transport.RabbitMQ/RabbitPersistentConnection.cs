using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Core.Messaging.Transport.RabbitMQ;

/// <summary>
/// Implements a persistent connection to RabbitMQ with automatic reconnection capabilities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the RabbitPersistentConnection class.
/// </remarks>
/// <param name="connectionFactory">The RabbitMQ connection factory.</param>
/// <param name="logger">Logger for connection-related events.</param>
/// <exception cref="ArgumentNullException">Thrown when connectionFactory or logger is null.</exception>
public sealed class RabbitPersistentConnection(IConnectionFactory connectionFactory, ILogger<RabbitPersistentConnection> logger) : IDisposable, IBusConnection
{
    private readonly ILogger<RabbitPersistentConnection> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    private IConnection _connection;
    private bool _disposed;



    private readonly object semaphore = new();

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
    private Task TryConnectAsync()
    {
        lock (semaphore)
        {
            if (IsConnected)
                return Task.CompletedTask;
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, timeSpan, context) =>
                    {
                        _logger.LogError(ex, $"An exception occurred while opening RabbitMQ connection: {ex.Message}");
                        return Task.CompletedTask;
                    });

            _connection = policy.ExecuteAsync(async () =>
                await _connectionFactory.CreateConnectionAsync()).GetAwaiter().GetResult();

            _connection.ConnectionShutdownAsync += OnConnectionShutdown;
            _connection.CallbackExceptionAsync += OnCallbackException;
            _connection.ConnectionBlockedAsync += OnConnectionBlocked;
        }

        return Task.CompletedTask;
    }


    private async Task OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection shutdown. Reason: {Reason}", e.ReplyText);
        await TryConnectAsync();
    }

    private async Task OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "RabbitMQ callback exception occurred.");
        await TryConnectAsync();
    }

    private async Task OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        _logger.LogWarning("RabbitMQ connection is blocked. Reason: {Reason}", e.Reason);
        await TryConnectAsync();
    }


    /// <summary>
    /// Creates a new channel for communication with RabbitMQ.
    /// </summary>
    /// <returns>A new channel instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no RabbitMQ connections are available.</exception>
    public async Task<IChannel> CreateChannelAsync()
    {
        await TryConnectAsync();

        if (!IsConnected)
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        return await _connection.CreateChannelAsync();
    }
}
