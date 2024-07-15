namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Dock
{
    public Guid Id { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public bool IsOccupied { get; private set; }
}