using MediatR;

namespace Core.Abstractions.CQRS.Command;

/// <summary>
/// Represents a command for creating a new entity.
/// </summary>
/// <typeparam name="TResponse">The type of response returned after creation.</typeparam>
public interface ICreateCommand<out TResponse> : ICommand<TResponse>
{
}

/// <summary>
/// Represents a command for creating a new entity without returning a value.
/// </summary>
public interface ICreateCommand : ICreateCommand<Unit>
{
}
