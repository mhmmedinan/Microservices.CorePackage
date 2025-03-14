namespace Core.Application.Responses;

/// <summary>
/// Generic response class for list operations.
/// Provides a standardized way to return collections of items.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class GetListResponse<T>
{
    /// <summary>
    /// Gets or sets the list of items.
    /// Initializes a new list if the items collection is null.
    /// </summary>
    public IList<T> Items
    {
        get => _items ??= new List<T>();
        set => _items = value;
    }

    private IList<T> _items;
}
