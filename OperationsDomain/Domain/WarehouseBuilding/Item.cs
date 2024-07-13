namespace OperationsDomain.Domain.WarehouseBuilding;

public sealed class Item
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    public bool CanBePicked() =>
        true;

    public bool Pick() =>
        true;
}