using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
}
