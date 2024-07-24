namespace Habanerio.Core;

public class PagedResults<T>
{
    /// <summary>
    /// The items for the current page of results.
    /// </summary>
    public IEnumerable<T> Items { get; set; }

    /// <summary>
    /// The current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of pages for the query.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// The total number of items in the query.
    /// </summary>
    public int TotalCount { get; set; }

    public PagedResults(IEnumerable<T>? items, int page, int pageSize, int totalCount)
    {
        Items = items ?? Enumerable.Empty<T>();
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}