namespace Core.Abstractions.Types;

/// <summary>
/// Defines a contract for resolving and registering types in the application.
/// Provides functionality to dynamically resolve types by name and register types for later resolution.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Dynamically resolve types at runtime</item>
/// <item>Register types for later resolution</item>
/// <item>Implement plugin or module systems</item>
/// <item>Handle dynamic type loading</item>
/// </list>
/// Implementation considerations:
/// <list type="bullet">
/// <item>Implementations should be thread-safe</item>
/// <item>Type resolution should be case-insensitive</item>
/// <item>Handle assembly loading appropriately</item>
/// <item>Consider caching resolved types</item>
/// </list>
/// </remarks>
public interface ITypeResolver
{
    /// <summary>
    /// Resolves a type by its name.
    /// </summary>
    /// <param name="typeName">The name of the type to resolve. Can be either the full name or just the type name.</param>
    /// <returns>The resolved Type object.</returns>
    /// <exception cref="TypeLoadException">Thrown when the type cannot be resolved.</exception>
    Type Resolve(string typeName);

    /// <summary>
    /// Registers a single type for later resolution.
    /// </summary>
    /// <param name="type">The type to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when the type parameter is null.</exception>
    void Register(Type type);

    /// <summary>
    /// Registers multiple types for later resolution.
    /// </summary>
    /// <param name="types">The list of types to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when the types parameter is null.</exception>
    void Register(IList<Type> types);
}