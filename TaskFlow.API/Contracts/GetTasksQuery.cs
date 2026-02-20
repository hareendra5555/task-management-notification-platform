using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.Contracts;

public class GetTasksQuery
{
    public bool? IsCompleted { get; set; }
    public bool? IsHighUrgency { get; set; }
    public string? Search { get; set; }
    public string SortBy { get; set; } = "priority";
    public string SortDirection { get; set; } = "desc";

    [Range(1, 10000)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}
