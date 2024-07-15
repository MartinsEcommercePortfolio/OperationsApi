using OperationsDomain.Warehouse.Employees;

namespace OperationsApi.Utilities;

internal static class HttpExtentions
{
    public static Employee Employee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return OperationsDomain.Warehouse.Employees.Employee.Null();
        return userObj as Employee ?? OperationsDomain.Warehouse.Employees.Employee.Null();
    }
}