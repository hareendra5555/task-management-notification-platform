using System.Collections.Generic;

namespace TaskFlow.API.Contracts;

public class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
