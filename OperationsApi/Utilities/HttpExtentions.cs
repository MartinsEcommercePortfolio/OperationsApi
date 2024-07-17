using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsApi.Utilities;

internal static class HttpExtentions
{
    public static Employee Employee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return OperationsDomain.Warehouse.Employees.Models.Employee.Null();
        return userObj as Employee ?? OperationsDomain.Warehouse.Employees.Models.Employee.Null();
    }
}