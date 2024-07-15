using OperationsDomain.Warehouse.Employees;
using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Racking
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
    public Pallet? Pallet { get; private set; }

    public bool IsOwnedBy( Employee employee ) =>
        Owner == employee;
    public bool IsAvailable() =>
        Owner is null &&
        Pallet is null;
    public bool CanBePickedFrom() =>
        Owner is null &&
        Pallet is not null &&
        Pallet.CanBePickedFrom();
    public bool PalletFits( Pallet pallet ) =>
        Length > pallet.Length &&
        Width > pallet.Width &&
        Height > pallet.Height;
    public bool TakePallet( Pallet pallet )
    {
        bool palletTaken = IsAvailable() &&
            PalletFits( pallet ) &&
            Capacity > pallet.Weight;
        if (palletTaken)
            Pallet = pallet;
        return palletTaken;
    }
    public bool AssignTo( Employee employee )
    {
        OwnerId = employee.Id;
        Owner = employee;
        return true;
    }
    public bool Free( Employee employee )
    {
        if (Owner != employee)
            return false;

        Owner = null;
        return true;
    }
}