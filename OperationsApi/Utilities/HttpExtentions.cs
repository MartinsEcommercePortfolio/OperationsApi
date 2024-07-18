using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Employees.Models.Variants;

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
            return new ReceivingEmployee();
        return userObj as ReceivingEmployee ?? new ReceivingEmployee();
    }
    public static PutawayEmployee GetPutawayEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return new PutawayEmployee();
        return userObj as PutawayEmployee ?? new PutawayEmployee();
    }
    public static PickingEmployee GetPickingEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return new PickingEmployee();
        return userObj as PickingEmployee ?? new PickingEmployee();
    }
    public static LoadingEmployee GetLoadingEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return new LoadingEmployee();
        return userObj as LoadingEmployee ?? new LoadingEmployee();
    }
}