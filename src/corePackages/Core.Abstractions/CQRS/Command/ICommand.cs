using MediatR;

namespace Core.Abstractions.CQRS.Command;

/// <summary>
/// Base interface for commands that don't return a value.
/// Represents a command in the CQRS pattern.
/// </summary>
public interface ICommand : ICommand<Unit>
{
}

/// <summary>
/// Base interface for commands that return a value.
/// </summary>
/// <typeparam name="TResponse">The type of result returned by the command.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}