using Microsoft.EntityFrameworkCore;
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
            .OrderByDescending(x => x.PriorityScore)
            .ThenBy(x => x.DueDate)
            .ThenByDescending(x => x.Created)
            .ToListAsync();
    }

    public async Task<PagedResponse<TaskItem>> GetPagedAsync(GetTasksQuery query)
    {
        var taskQuery = _context.Tasks.AsQueryable();

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

        var totalCount = await taskQuery.CountAsync();

        var items = await taskQuery
            .OrderByDescending(x => x.PriorityScore)
            .ThenBy(x => x.DueDate)
            .ThenByDescending(x => x.Created)
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
}
