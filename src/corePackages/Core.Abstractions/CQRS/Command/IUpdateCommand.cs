using MediatR;

namespace Core.Abstractions.CQRS.Command;

/// <summary>
/// Represents a command for updating an existing entity.
/// </summary>
/// <typeparam name="TResponse">The type of response returned after update.</typeparam>
public interface IUpdateCommand<out TResponse> : ICommand<TResponse>
{
}

/// <summary>
/// Represents a command for updating an existing entity without returning a value.
/// </summary>
public interface IUpdateCommand : IUpdateCommand<Unit>
{
}
