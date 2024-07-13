namespace OperationsApi.Domain.Warehouses;

internal sealed class Dock
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
}