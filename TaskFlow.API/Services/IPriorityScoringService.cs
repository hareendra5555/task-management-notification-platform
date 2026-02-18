using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public interface IPriorityScoringService
{
    int CalculateScore(TaskItem task);
}
