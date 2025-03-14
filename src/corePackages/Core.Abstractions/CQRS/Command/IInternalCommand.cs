namespace Core.Abstractions.CQRS.Command;

/// <summary>
/// Represents a command that is processed internally within the system.
/// </summary>
public interface IInternalCommand : ICommand
{
    /// <summary>
    /// Gets the unique identifier for the command.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the date and time when the command occurred.
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the type name of the command.
    /// </summary>
    string CommandType { get; }
}