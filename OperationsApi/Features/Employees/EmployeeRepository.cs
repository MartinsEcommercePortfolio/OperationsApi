using Microsoft.EntityFrameworkCore;
using OperationsApi.Database;
using OperationsApi.Domain.Employees;

namespace OperationsApi.Features.Employees;

internal sealed class EmployeeRepository( WarehouseDbContext dbContext, ILogger<EmployeeRepository> logger ) 
    : DatabaseService<EmployeeRepository>( dbContext, logger )
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