namespace Core.Abstractions.Types;

public interface ITypeResolver
{
    Type Resolve(string typeName);
    void Register(Type type);
    void Register(IList<Type> types);
}