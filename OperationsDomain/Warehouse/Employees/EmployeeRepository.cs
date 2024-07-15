using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationsDomain._Database;

namespace OperationsDomain.Warehouse.Employees;

internal sealed class EmployeeRepository( WarehouseDbContext dbContext, ILogger<EmployeeRepository> logger ) 
    : DatabaseService<EmployeeRepository>( dbContext, logger ), IEmployeeRepository
{
    public async Task<Employee?> GetEmployeeById( Guid id )
    {
        try
        {
            return await DbContext.Employees
                .FirstOrDefaultAsync( e => e.Id == id );
        }
        catch ( Exception e )
        {
            return ProcessDbException<Employee?>( e, null );
        }
    }
}