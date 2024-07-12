using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Utilities;

internal static class HttpExtentions
{
    public static Employee Employee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return Domain.Warehouses.Employee.Null();
        return userObj as Employee ?? Domain.Warehouses.Employee.Null();
    }
}