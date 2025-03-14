public abstract record InternalCommand : IInternalCommand
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime OccurredOn { get; protected set; } = DateTime.Now;

    public string CommandType { get { return GetType().AssemblyQualifiedName; } }
}
