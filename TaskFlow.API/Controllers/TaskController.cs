using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.API.Contracts;
using TaskFlow.API.UnitOfWork;
using TaskFlow.API.Models;
using TaskFlow.API.Services;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPriorityScoringService _priorityScoringService;
    private readonly INotificationService _notificationService;
    private readonly ITaskCacheService _taskCacheService;

    public TasksController(
        IUnitOfWork unitOfWork,
        IPriorityScoringService priorityScoringService,
        INotificationService notificationService,
        ITaskCacheService taskCacheService)
    {
        _unitOfWork = unitOfWork;
        _priorityScoringService = priorityScoringService;
        _notificationService = notificationService;
        _taskCacheService = taskCacheService;
    }

    [HttpGet(Name = "GetAllTasks")]
    public async Task<IActionResult> GetAllTasks([FromQuery] GetTasksQuery query)
    {
        var isDefaultQuery = !query.IsCompleted.HasValue
            && !query.IsHighUrgency.HasValue
            && !query.DueFrom.HasValue
            && !query.DueTo.HasValue
            && string.IsNullOrWhiteSpace(query.Search)
            && IsDefaultSort(query.SortBy, query.SortDirection);

        if (isDefaultQuery)
        {
            var cachedTasks = await _taskCacheService.GetTaskListAsync();
            IReadOnlyCollection<TaskItem> allTasks;

            if (cachedTasks is not null)
            {
                allTasks = cachedTasks;
            }
            else
            {
                var fetchedTasks = (await _unitOfWork.Tasks.GetAllOrderedAsync()).ToList();
                await _taskCacheService.SetTaskListAsync(fetchedTasks);
                allTasks = fetchedTasks;
            }

            var pagedItems = allTasks
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Ok(new PagedResponse<TaskItem>
            {
                Items = pagedItems,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = allTasks.Count
            });
        }

        var result = await _unitOfWork.Tasks.GetPagedAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var cachedTask = await _taskCacheService.GetTaskAsync(id);
        if (cachedTask is not null)
        {
            return Ok(cachedTask);
        }

        var task = await _unitOfWork.Tasks.GetByIdAsync(id);

        if (task is null)
        {
            return NotFound();
        }

        await _taskCacheService.SetTaskAsync(task);
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            IsHighUrgency = request.IsHighUrgency,
            IsCompleted = false
        };

        task.PriorityScore = _priorityScoringService.CalculateScore(task);

        await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.CompleteAsync();
        await _taskCacheService.InvalidateTaskListAsync();

        _notificationService.Publish(new NotificationEvent
        {
            TaskId = task.Id,
            EventType = "TaskCreated",
            Message = $"Task '{task.Title}' has been created."
        });

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskRequest request)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        if (task is null)
        {
            return NotFound();
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.IsCompleted = request.IsCompleted;
        task.IsHighUrgency = request.IsHighUrgency;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;
        task.PriorityScore = _priorityScoringService.CalculateScore(task);

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.CompleteAsync();

        await _taskCacheService.InvalidateTaskListAsync();
        await _taskCacheService.InvalidateTaskAsync(id);

        _notificationService.Publish(new NotificationEvent
        {
            TaskId = task.Id,
            EventType = "TaskUpdated",
            Message = $"Task '{task.Title}' has been updated."
        });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id);
        if (task == null) return NotFound();

        _unitOfWork.Tasks.Delete(task);
        await _unitOfWork.CompleteAsync();

        await _taskCacheService.InvalidateTaskListAsync();
        await _taskCacheService.InvalidateTaskAsync(id);

        _notificationService.Publish(new NotificationEvent
        {
            TaskId = task.Id,
            EventType = "TaskDeleted",
            Message = $"Task '{task.Title}' has been deleted."
        });

        return NoContent();
    }

    [HttpGet("notifications")]
    public IActionResult GetRecentNotifications([FromQuery] int count = 20)
    {
        var normalizedCount = Math.Clamp(count, 1, 100);
        return Ok(_notificationService.GetRecent(normalizedCount));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetTaskSummary()
    {
        var summary = await _unitOfWork.Tasks.GetSummaryAsync();
        return Ok(summary);
    }

    private static bool IsDefaultSort(string? sortBy, string? sortDirection)
    {
        var isDefaultSortBy = string.IsNullOrWhiteSpace(sortBy)
            || string.Equals(sortBy.Trim(), "priority", StringComparison.OrdinalIgnoreCase);

        var isDefaultSortDirection = string.IsNullOrWhiteSpace(sortDirection)
            || string.Equals(sortDirection.Trim(), "desc", StringComparison.OrdinalIgnoreCase);

        return isDefaultSortBy && isDefaultSortDirection;
    }
}
