using OperationsDomain.Warehouses.Employees;

namespace OperationsApi.Middleware;

internal sealed class EmployeeAuthenticationMiddleware( RequestDelegate next )
{
    readonly RequestDelegate _next = next;

    public async Task InvokeAsync( HttpContext context, IEmployeeRepository employees )
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

        Employee? user = await employees.GetEmployeeById( userId );
        
        if (user is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }
        
        context.Items["User"] = user;
        await _next( context );
    }
}