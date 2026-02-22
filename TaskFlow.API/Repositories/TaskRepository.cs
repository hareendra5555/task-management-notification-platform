using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskFlow.API.Contracts;
using TaskFlow.API.Data;
using TaskFlow.API.Models;

namespace TaskFlow.API.Repositories;

public class TaskRepository : GenericRepository<TaskItem>, ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAllOrderedAsync()
    {
        return await _context.Tasks
            .AsNoTracking()
            .OrderByDescending(x => x.PriorityScore)
            .ThenBy(x => x.DueDate)
            .ThenByDescending(x => x.Created)
            .ToListAsync();
    }

    public async Task<PagedResponse<TaskItem>> GetPagedAsync(GetTasksQuery query)
    {
        var taskQuery = _context.Tasks
            .AsNoTracking()
            .AsQueryable();

        if (query.IsCompleted.HasValue)
        {
            taskQuery = taskQuery.Where(x => x.IsCompleted == query.IsCompleted.Value);
        }

        if (query.IsHighUrgency.HasValue)
        {
            taskQuery = taskQuery.Where(x => x.IsHighUrgency == query.IsHighUrgency.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchTerm = query.Search.Trim();
            taskQuery = taskQuery.Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm));
        }

        if (query.DueFrom.HasValue)
        {
            taskQuery = taskQuery.Where(x => x.DueDate.HasValue && x.DueDate.Value >= query.DueFrom.Value);
        }

        if (query.DueTo.HasValue)
        {
            taskQuery = taskQuery.Where(x => x.DueDate.HasValue && x.DueDate.Value <= query.DueTo.Value);
        }

        if (query.MinPriorityScore.HasValue)
        {
            taskQuery = taskQuery.Where(x => x.PriorityScore >= query.MinPriorityScore.Value);
        }

        if (query.MaxPriorityScore.HasValue)
        {
            taskQuery = taskQuery.Where(x => x.PriorityScore <= query.MaxPriorityScore.Value);
        }

        if (query.CreatedFrom.HasValue)
        {
            taskQuery = taskQuery.Where(x => x.Created >= query.CreatedFrom.Value);
        }

        if (query.CreatedTo.HasValue)
        {
            taskQuery = taskQuery.Where(x => x.Created <= query.CreatedTo.Value);
        }

        var totalCount = await taskQuery.CountAsync();

        var sortedQuery = ApplySorting(taskQuery, query.SortBy, query.SortDirection);

        var items = await sortedQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResponse<TaskItem>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TaskSummaryResponse> GetSummaryAsync()
    {
        var now = DateTime.UtcNow;
        var tasksQuery = _context.Tasks.AsNoTracking();

        return new TaskSummaryResponse
        {
            TotalTasks = await tasksQuery.CountAsync(),
            CompletedTasks = await tasksQuery.CountAsync(x => x.IsCompleted),
            PendingTasks = await tasksQuery.CountAsync(x => !x.IsCompleted),
            HighUrgencyTasks = await tasksQuery.CountAsync(x => x.IsHighUrgency),
            OverdueTasks = await tasksQuery.CountAsync(x => !x.IsCompleted && x.DueDate.HasValue && x.DueDate.Value < now)
        };
    }

    private static IQueryable<TaskItem> ApplySorting(IQueryable<TaskItem> query, string? sortBy, string? sortDirection)
    {
        var normalizedSortBy = sortBy?.Trim().ToLowerInvariant() ?? "priority";
        var isAscending = string.Equals(sortDirection?.Trim(), "asc", StringComparison.OrdinalIgnoreCase);

        return normalizedSortBy switch
        {
            "duedate" => isAscending
                ? query.OrderBy(x => x.DueDate).ThenByDescending(x => x.PriorityScore)
                : query.OrderByDescending(x => x.DueDate).ThenByDescending(x => x.PriorityScore),
            "created" => isAscending
                ? query.OrderBy(x => x.Created).ThenByDescending(x => x.PriorityScore)
                : query.OrderByDescending(x => x.Created).ThenByDescending(x => x.PriorityScore),
            "title" => isAscending
                ? query.OrderBy(x => x.Title).ThenByDescending(x => x.PriorityScore)
                : query.OrderByDescending(x => x.Title).ThenByDescending(x => x.PriorityScore),
            _ => isAscending
                ? query.OrderBy(x => x.PriorityScore).ThenBy(x => x.DueDate).ThenByDescending(x => x.Created)
                : query.OrderByDescending(x => x.PriorityScore).ThenBy(x => x.DueDate).ThenByDescending(x => x.Created)
        };
    }
}
