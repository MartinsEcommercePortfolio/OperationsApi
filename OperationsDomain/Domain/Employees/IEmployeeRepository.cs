using OperationsDomain.Database;

namespace OperationsDomain.Domain.Employees;

public interface IEmployeeRepository : IEfCoreRepository
{
    public Task<Employee?> GetEmployeeById( Guid id );
}