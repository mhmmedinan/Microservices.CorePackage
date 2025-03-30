using Core.Abstractions.Events;
using Core.Abstractions.Scheduler;
using System.Reflection;

namespace Core.Abstractions.Extensions;

/// <summary>
/// Provides extension methods to simplify reflection-based type queries and operations.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets all types implementing a specified open generic interface from provided assemblies.
    /// </summary>
    /// <param name="openGenericType">The open generic interface type (e.g., typeof(IMyInterface&lt;&gt;)).</param>
    /// <param name="assemblies">Optional assemblies to search; defaults to currently loaded assemblies if none provided.</param>
    /// <returns>A collection of types implementing the specified open generic interface.</returns>
    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
        this Type openGenericType,
        params Assembly[] assemblies)
    {
        var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
        return inputAssemblies.SelectMany(assembly =>
            GetAllTypesImplementingOpenGenericInterface(openGenericType, assembly));
    }

    /// <summary>
    /// Gets all types implementing a specified open generic interface from a single assembly.
    /// </summary>
    /// <param name="openGenericType">The open generic interface type.</param>
    /// <param name="assembly">The assembly to search.</param>
    /// <returns>A collection of types implementing the specified open generic interface.</returns>
    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
        this Type openGenericType,
        Assembly assembly)
    {
        try
        {
            return GetAllTypesImplementingOpenGenericInterface(openGenericType, assembly.GetTypes());
        }
        catch (ReflectionTypeLoadException)
        {
            return Enumerable.Empty<Type>();
        }
    }

    /// <summary>
    /// Gets all types implementing a specified open generic interface from a given type collection.
    /// </summary>
    /// <param name="openGenericType">The open generic interface type.</param>
    /// <param name="types">The types to search.</param>
    /// <returns>A collection of types implementing the specified open generic interface.</returns>
    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
        this Type openGenericType,
        IEnumerable<Type> types)
    {
        return from type in types
               from interfaceType in type.GetInterfaces()
               where interfaceType.IsGenericType
                     && openGenericType.IsAssignableFrom(interfaceType.GetGenericTypeDefinition())
                     && type.IsClass
                     && !type.IsAbstract
               select type;
    }

    /// <summary>
    /// Gets all types implementing a specified interface from provided assemblies.
    /// </summary>
    /// <param name="interfaceType">The interface type.</param>
    /// <param name="assemblies">Optional assemblies to search; defaults to currently loaded assemblies if none provided.</param>
    /// <returns>A collection of types implementing the specified interface.</returns>
    public static IEnumerable<Type> GetAllTypesImplementingInterface(
        this Type interfaceType,
        params Assembly[] assemblies)
    {
        var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
        return inputAssemblies.SelectMany(assembly => assembly.GetTypes()
            .Where(type => interfaceType.IsAssignableFrom(type)
                           && type.IsClass
                           && !type.IsAbstract));
    }

    /// <summary>
    /// Resolves and returns the payload type from a given <see cref="ScheduleSerializedObject"/>.
    /// </summary>
    /// <param name="messageSerializedObject">The serialized object containing type information.</param>
    /// <returns>The resolved payload type, or null if not found.</returns>
    public static Type GetPayloadType(this ScheduleSerializedObject messageSerializedObject)
    {
        if (messageSerializedObject?.AssemblyName == null)
            return null;

        var assembly = Assembly.Load(messageSerializedObject.AssemblyName);

        return assembly.GetTypes()
            .FirstOrDefault(t => t.FullName == messageSerializedObject.TypeName);
    }

    /// <summary>
    /// Determines whether the specified type implements the <see cref="IEvent"/> interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the type implements <see cref="IEvent"/>; otherwise, <c>false</c>.</returns>
    public static bool IsEvent(this Type type)
        => type.IsAssignableTo(typeof(IEvent));
}