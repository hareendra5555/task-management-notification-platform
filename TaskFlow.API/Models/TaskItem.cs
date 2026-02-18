using System;

namespace TaskFlow.API.Models;

public class TaskItem : BaseModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsHighUrgency { get; set; }
    public int PriorityScore { get; set; }
}
