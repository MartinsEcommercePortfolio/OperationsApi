using OperationsApi.Domain.Warehouses;

namespace OperationsApi.Utilities;

internal static class HttpExtentions
{
    public static Employee GetEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return Employee.Null();
        return userObj as Employee ?? Employee.Null();
    }
}