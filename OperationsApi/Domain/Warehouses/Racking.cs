using OperationsApi.Domain.Employees;

namespace OperationsApi.Domain.Warehouses;

internal sealed class Racking
{
    public Guid Id { get; private set; }
    public string Aisle { get; private set; } = string.Empty;
    public string Bay { get; private set; } = string.Empty;
    public string Level { get; private set; } = string.Empty;
    public double Length { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double Capacity { get; private set; }
    public Guid OwnerId { get; private set; }
    public Employee? Owner { get; private set; }
    public Pallet? Pallet { get; set; }

    public bool IsOwnedBy( Employee employee ) =>
        Owner == employee;
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