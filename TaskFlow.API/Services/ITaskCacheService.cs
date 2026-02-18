using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public interface ITaskCacheService
{
    Task<IReadOnlyCollection<TaskItem>?> GetTaskListAsync();
    Task SetTaskListAsync(IEnumerable<TaskItem> tasks);
    Task<TaskItem?> GetTaskAsync(Guid taskId);
    Task SetTaskAsync(TaskItem task);
    Task InvalidateTaskListAsync();
    Task InvalidateTaskAsync(Guid taskId);
}
