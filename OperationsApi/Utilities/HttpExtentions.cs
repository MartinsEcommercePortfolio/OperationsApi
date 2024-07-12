using OperationsApi.Domain.Employees;

namespace OperationsApi.Utilities;

internal static class HttpExtentions
{
    public static Employee Employee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return Domain.Employees.Employee.Null();
        return userObj as Employee ?? Domain.Employees.Employee.Null();
    }
}