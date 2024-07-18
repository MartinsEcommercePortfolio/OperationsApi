namespace OperationsDomain.Ordering.Models;

public sealed class WarehouseOrderItem
{
    public WarehouseOrderItem( Guid productId, int quantity )
    {
        ProductId = productId;
        Quantity = quantity;
    }
    
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
}