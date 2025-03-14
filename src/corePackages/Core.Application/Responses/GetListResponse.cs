namespace Core.Application.Responses;

public class GetListResponse<T>
{
    public IList<T> Items
    {
        get => _items ??= new List<T>();
        set => _items = value;
    }

    private IList<T> _items;
}
