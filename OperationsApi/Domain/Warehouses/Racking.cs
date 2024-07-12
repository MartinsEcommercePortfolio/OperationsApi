namespace OperationsApi.Domain.Warehouses;

internal sealed class Racking
{
    public Guid Id { get; set; }
    public string Aisle { get; set; } = string.Empty;
    public string Bay { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Capacity { get; set; }
    public Guid OwnerId { get; set; }
    public Employee? Owner { get; set; }

    public bool CanHoldPallet( Pallet pallet ) =>
        Length > pallet.Length &&
        Width > pallet.Width &&
        Height > pallet.Height &&
        Capacity > pallet.Weight;
    public void AssignTo( Employee employee )
    {
        OwnerId = employee.Id;
        Owner = employee;
    }
}