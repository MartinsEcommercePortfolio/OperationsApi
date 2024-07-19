using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsApi.Utilities;

internal static class HttpExtentions
{
    public static Employee GetEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return Employee.Null();
        return userObj as Employee ?? Employee.Null();
    }
    public static ReceivingEmployee GetReceivingEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return (ReceivingEmployee) Employee.Null();
        return userObj as ReceivingEmployee ?? (ReceivingEmployee) Employee.Null();
    }
    public static PutawayEmployee GetPutawayEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return (PutawayEmployee) Employee.Null();
        return userObj as PutawayEmployee ?? (PutawayEmployee) Employee.Null();
    }
    public static PickingEmployee GetPickingEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return (PickingEmployee) Employee.Null();
        return userObj as PickingEmployee ?? (PickingEmployee) Employee.Null();
    }
    public static LoadingEmployee GetLoadingEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return (LoadingEmployee) Employee.Null();
        return userObj as LoadingEmployee ?? (LoadingEmployee) Employee.Null();
    }
}