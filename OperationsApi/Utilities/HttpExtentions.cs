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
            return new ReceivingEmployee( string.Empty );
        return userObj as ReceivingEmployee ?? new ReceivingEmployee( string.Empty );
    }
    public static PutawayEmployee GetPutawayEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return new PutawayEmployee( string.Empty );
        return userObj as PutawayEmployee ?? new PutawayEmployee( string.Empty );
    }
    public static PickingEmployee GetPickingEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return new PickingEmployee( string.Empty );
        return userObj as PickingEmployee ?? new PickingEmployee( string.Empty );
    }
    public static LoadingEmployee GetLoadingEmployee( this HttpContext http )
    {
        if (!http.Items.TryGetValue( "Employee", out var userObj ))
            return new LoadingEmployee( string.Empty );
        return userObj as LoadingEmployee ?? new LoadingEmployee( string.Empty );
    }
}