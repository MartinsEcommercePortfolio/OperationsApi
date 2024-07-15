using OperationsDomain.Warehouses.Employees;

namespace OperationsApi.Utilities;

internal static class HttpExtentions
{
    public static Employee Employee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return OperationsDomain.Warehouses.Employees.Employee.Null();
        return userObj as Employee ?? OperationsDomain.Warehouses.Employees.Employee.Null();
    }
}