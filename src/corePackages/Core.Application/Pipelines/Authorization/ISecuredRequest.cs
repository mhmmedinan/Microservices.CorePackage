namespace Core.Application.Pipelines.Authorization;

/// <summary>
/// Interface for requests that require authorization.
/// Implements role-based security for MediatR pipeline requests.
/// </summary>
public interface ISecuredRequest
{
    /// <summary>
    /// Gets the array of role names that are authorized to execute this request.
    /// </summary>
    public string[] Roles { get; }
}
