namespace OperationsDomain.Ordering.Models;

public sealed class WarehouseOrderItem
{
    WarehouseOrderItem( Guid productId, int quantity )
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public static WarehouseOrderItem New( Guid productId, int quantity ) =>
        new( productId, quantity );
    
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
}