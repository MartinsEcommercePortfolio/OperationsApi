using OperationsDomain._Database;
using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Employees;

public interface IEmployeeRepository : IEfCoreRepository
{
    public Task<Employee?> GetEmployeeById( Guid id );
}