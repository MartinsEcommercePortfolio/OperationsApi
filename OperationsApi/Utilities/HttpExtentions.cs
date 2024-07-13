using OperationsDomain.Domain.Employees;

namespace OperationsApi.Utilities;

internal static class HttpExtentions
{
    public static Employee Employee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return OperationsDomain.Domain.Employees.Employee.Null();
        return userObj as Employee ?? OperationsDomain.Domain.Employees.Employee.Null();
    }
}