using System;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class PriorityScoringService : IPriorityScoringService
{
    public int CalculateScore(TaskItem task)
    {
        var score = 10;

        if (task.IsHighUrgency)
        {
            score += 25;
        }

        if (task.DueDate.HasValue)
        {
            var now = DateTime.UtcNow;

            if (task.DueDate.Value < now)
            {
                score += 35;
            }
            else if (task.DueDate.Value <= now.AddDays(1))
            {
                score += 20;
            }
            else if (task.DueDate.Value <= now.AddDays(3))
            {
                score += 10;
            }
        }

        if (task.IsCompleted)
        {
            score = Math.Max(0, score - 40);
        }

        return score;
    }
}
