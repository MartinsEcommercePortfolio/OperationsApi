namespace OperationsApi.Types.Warehouses;

internal sealed class Dock
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public bool IsOccupied { get; set; }
}