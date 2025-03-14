namespace Core.Abstractions.Scheduler;

/// <summary>
/// Represents a serialized object for scheduling tasks.
/// </summary>
public class ScheduleSerializedObject
{
    /// <summary>
    /// Gets or sets the type name of the scheduled object.
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// Gets or sets the assembly name containing the scheduled object type.
    /// </summary>
    public string AssemblyName { get; set; }

    /// <summary>
    /// Gets or sets the serialized data of the scheduled object.
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleSerializedObject"/> class.
    /// </summary>
    /// <param name="typeName">The type name of the scheduled object.</param>
    /// <param name="assemblyName">The assembly name containing the scheduled object type.</param>
    /// <param name="data">The serialized data of the scheduled object.</param>
    public ScheduleSerializedObject(string typeName, string assemblyName, string data)
    {
        TypeName = typeName;
        AssemblyName = assemblyName;
        Data = data;
    }

    public override string ToString()
    {
        var commandName = TypeName.Split('.').Last();
        return $"{commandName}";
    }
}
