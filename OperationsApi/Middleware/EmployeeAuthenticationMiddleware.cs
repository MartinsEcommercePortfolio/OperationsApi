using OperationsApi.Database;
using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Middleware;

internal sealed class EmployeeAuthenticationMiddleware( RequestDelegate next )
{
    readonly RequestDelegate _next = next;

    public async Task InvokeAsync( HttpContext context, WarehouseDbContext dbContext )
    {
        Guid userId = Guid.Empty;
        bool validUrl =
            context.Request.Query.TryGetValue( "UserId", out var guidString ) &&
            Guid.TryParse( guidString, out userId );
        
        if (!validUrl)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        Employee? user = await dbContext.Employees.FindAsync( userId );
        
        if (user is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }
        
        context.Items["User"] = user;
        await _next( context );
    }
}