using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class TaskCacheService : ITaskCacheService
{
    private const string AllTasksKey = "tasks:all";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = CacheDuration
    };

    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public TaskCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<IReadOnlyCollection<TaskItem>?> GetTaskListAsync()
    {
        var json = await _cache.GetStringAsync(AllTasksKey);
        return json is null ? null : JsonSerializer.Deserialize<IReadOnlyCollection<TaskItem>>(json, _jsonOptions);
    }

    public Task SetTaskListAsync(IEnumerable<TaskItem> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, _jsonOptions);
        return _cache.SetStringAsync(AllTasksKey, json, CacheOptions);
    }

    public async Task<TaskItem?> GetTaskAsync(Guid taskId)
    {
        var json = await _cache.GetStringAsync(GetTaskKey(taskId));
        return json is null ? null : JsonSerializer.Deserialize<TaskItem>(json, _jsonOptions);
    }

    public Task SetTaskAsync(TaskItem task)
    {
        var json = JsonSerializer.Serialize(task, _jsonOptions);
        return _cache.SetStringAsync(GetTaskKey(task.Id), json, CacheOptions);
    }

    public Task InvalidateTaskListAsync() => _cache.RemoveAsync(AllTasksKey);

    public Task InvalidateTaskAsync(Guid taskId) => _cache.RemoveAsync(GetTaskKey(taskId));

    private static string GetTaskKey(Guid taskId) => $"tasks:{taskId}";
}
