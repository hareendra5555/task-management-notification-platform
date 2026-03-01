using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.Services;

public class TaskCacheOptions
{
    public const string SectionName = "TaskCache";

    [Range(1, 120)]
    public int DurationMinutes { get; set; } = 5;
}
