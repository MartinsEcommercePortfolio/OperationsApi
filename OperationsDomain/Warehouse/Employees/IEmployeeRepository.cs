using OperationsDomain._Database;

namespace OperationsDomain.Warehouse.Employees;

public interface IEmployeeRepository : IEfCoreRepository
{
    public Task<Employee?> GetEmployeeById( Guid id );
}