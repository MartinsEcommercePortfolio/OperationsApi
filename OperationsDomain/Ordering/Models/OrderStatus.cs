namespace OperationsDomain.Ordering.Models;

public enum OrderStatus
{
    Pending,
    Fulfilling,
    Shipping,
    Complete,
    Returned
}