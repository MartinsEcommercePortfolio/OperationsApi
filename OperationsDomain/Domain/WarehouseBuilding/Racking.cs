using OperationsDomain.Domain.Employees;

namespace OperationsDomain.Domain.WarehouseBuilding;

public sealed class Racking
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
    public Pallet? Pallet { get; set; }

    public bool IsOwnedBy( Employee employee ) =>
        Owner == employee;
    public bool IsAvailable() =>
        Owner is null &&
        Pallet is null;
    public bool PalletFits( Pallet pallet ) =>
        Length > pallet.Length &&
        Width > pallet.Width &&
        Height > pallet.Height;
    public bool TakePallet( Pallet pallet )
    {
        bool canTake = IsAvailable() &&
            PalletFits( pallet ) &&
            Capacity > pallet.Weight;
        if (canTake)
            Pallet = pallet;
        return canTake;
    }

    public void AssignTo( Employee employee )
    {
        OwnerId = employee.Id;
        Owner = employee;
    }
}