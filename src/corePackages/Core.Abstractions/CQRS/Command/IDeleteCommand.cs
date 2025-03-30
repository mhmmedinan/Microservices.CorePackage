using MediatR;

namespace Core.Abstractions.CQRS.Command;

/// <summary>
/// Represents a command for deleting an existing entity.
/// </summary>
/// <typeparam name="TResponse">The type of response returned after deletion.</typeparam>
public interface IDeleteCommand<out TResponse> : ICommand<TResponse>
{
}

/// <summary>
/// Represents a command for deleting an existing entity without returning a value.
/// </summary>
public interface IDeleteCommand : IDeleteCommand<Unit>
{
}
