using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.Contracts;

public class GetTasksQuery
{
    public bool? IsCompleted { get; set; }
    public bool? IsHighUrgency { get; set; }
    public string? Search { get; set; }

    [Range(1, 10000)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}
