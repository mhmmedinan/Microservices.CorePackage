namespace Core.Application.Requests;

/// <summary>
/// Represents a request for paginated data.
/// Contains parameters for pagination such as page index and size.
/// </summary>
public class PageRequest
{
    /// <summary>
    /// Gets or sets the zero-based page index.
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Gets or sets the size of each page.
    /// </summary>
    public int PageSize { get; set; }
}
