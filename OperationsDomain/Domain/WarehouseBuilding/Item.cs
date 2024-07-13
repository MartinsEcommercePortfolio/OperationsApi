namespace OperationsDomain.Domain.WarehouseBuilding;

public sealed class Item
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Stock { get; set; }
}