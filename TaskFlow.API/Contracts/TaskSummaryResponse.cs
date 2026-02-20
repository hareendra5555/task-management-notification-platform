namespace TaskFlow.API.Contracts;

public class TaskSummaryResponse
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int HighUrgencyTasks { get; set; }
    public int OverdueTasks { get; set; }
}
