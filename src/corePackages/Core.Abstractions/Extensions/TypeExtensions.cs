using Core.Abstractions.Events;
using Core.Abstractions.Scheduler;
using System.Reflection;

namespace Core.Abstractions.Extensions;

public static class TypeExtensions
{
    // https://stackoverflow.com/questions/42245011/get-all-implementations-types-of-a-generic-interface
    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
        this Type openGenericType,
        params Assembly[] assemblies)
    {
        var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
        return inputAssemblies.SelectMany(assembly =>
            GetAllTypesImplementingOpenGenericInterface(openGenericType, assembly));
    }

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
            // It's expected to not being able to load all assemblies
            return new List<Type>();
        }
    }

    public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
        this Type openGenericType,
        IEnumerable<Type> types)
    {
        return from type in types
               from interfaceType in type.GetInterfaces()
               where
                   interfaceType.IsGenericType &&
                   openGenericType.IsAssignableFrom(interfaceType.GetGenericTypeDefinition()) &&
                   type.IsClass && !type.IsAbstract
               select type;
    }

    // https://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
    public static IEnumerable<Type> GetAllTypesImplementingInterface(
        this Type interfaceType,
        params Assembly[] assemblies)
    {
        var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
        return inputAssemblies.SelectMany(assembly => GetAllTypesImplementingInterface(interfaceType, assembly));
    }

    private static IEnumerable<Type> GetAllTypesImplementingInterface(this Type interfaceType, Assembly assembly)
    {
        return assembly.GetTypes().Where(type =>
            interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract &&
            type.IsClass);
    }

    public static Type GetPayloadType(this ScheduleSerializedObject messageSerializedObject)
    {
        if (messageSerializedObject?.AssemblyName == null)
            return null;

        var assembly = Assembly.Load(messageSerializedObject.AssemblyName);

        var type = assembly
            .GetTypes()
            .Where(t => t.FullName == messageSerializedObject.FullTypeName)
            .ToList().FirstOrDefault();
        return type;
    }

    public static bool IsEvent(this Type type)
       => type.IsAssignableTo(typeof(IEvent));
}
