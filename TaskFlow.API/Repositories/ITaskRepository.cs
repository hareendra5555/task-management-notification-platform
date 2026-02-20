using TaskFlow.API.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using TaskFlow.API.Contracts;

namespace TaskFlow.API.Repositories;

public interface ITaskRepository : IGenericRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetAllOrderedAsync();
    Task<PagedResponse<TaskItem>> GetPagedAsync(GetTasksQuery query);
    Task<TaskSummaryResponse> GetSummaryAsync();
}
