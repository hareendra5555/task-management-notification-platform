using System;
using System.Threading.Tasks;   
using TaskFlow.API.Repositories;

namespace TaskFlow.API.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    ITaskRepository Tasks { get; }
    Task<int> CompleteAsync();
}