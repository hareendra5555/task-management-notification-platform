using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TaskFlow.API.Models;

namespace TaskFlow.API.Services;

public class TaskCacheService : ITaskCacheService
{
    private const string AllTasksKey = "tasks:all";

    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private readonly DistributedCacheEntryOptions _cacheOptions;

    public TaskCacheService(IDistributedCache cache, IOptions<TaskCacheOptions> cacheOptions)
    {
        _cache = cache;
        _cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheOptions.Value.DurationMinutes)
        };
    }

    public async Task<IReadOnlyCollection<TaskItem>?> GetTaskListAsync()
    {
        var json = await _cache.GetStringAsync(AllTasksKey);
        return json is null ? null : JsonSerializer.Deserialize<IReadOnlyCollection<TaskItem>>(json, _jsonOptions);
    }

    public Task SetTaskListAsync(IEnumerable<TaskItem> tasks)
    {
        var json = JsonSerializer.Serialize(tasks, _jsonOptions);
        return _cache.SetStringAsync(AllTasksKey, json, _cacheOptions);
    }

    public async Task<TaskItem?> GetTaskAsync(Guid taskId)
    {
        var json = await _cache.GetStringAsync(GetTaskKey(taskId));
        return json is null ? null : JsonSerializer.Deserialize<TaskItem>(json, _jsonOptions);
    }

    public Task SetTaskAsync(TaskItem task)
    {
        var json = JsonSerializer.Serialize(task, _jsonOptions);
        return _cache.SetStringAsync(GetTaskKey(task.Id), json, _cacheOptions);
    }

    public Task InvalidateTaskListAsync() => _cache.RemoveAsync(AllTasksKey);

    public Task InvalidateTaskAsync(Guid taskId) => _cache.RemoveAsync(GetTaskKey(taskId));

    private static string GetTaskKey(Guid taskId) => $"tasks:{taskId}";
}
