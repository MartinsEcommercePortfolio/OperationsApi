namespace OperationsDomain.Warehouses.Infrastructure;

public sealed class Dock
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
}