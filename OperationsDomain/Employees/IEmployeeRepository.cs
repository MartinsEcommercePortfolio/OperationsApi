using OperationsDomain._Database;
using OperationsDomain.Employees.Models;

namespace OperationsDomain.Employees;

public interface IEmployeeRepository : IEfCoreRepository
{
    public Task<Employee?> GetEmployeeById( Guid id );
}