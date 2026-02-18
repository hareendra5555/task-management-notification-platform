using TaskFlow.API.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TaskFlow.API.Repositories;

public interface ITaskRepository : IGenericRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetAllOrderedAsync();
}
