using System.Threading.Tasks;
using TaskFlow.API.Data;
using TaskFlow.API.Repositories;

namespace TaskFlow.API.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public ITaskRepository Tasks { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Tasks = new TaskRepository(context);
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}