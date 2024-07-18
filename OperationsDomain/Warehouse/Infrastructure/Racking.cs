using OperationsDomain.Catalog;
using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Racking
{
    public Racking(
        bool isPickSlot,
        string aisle,
        string bay,
        string level,
        double length,
        double width,
        double height,
        double capacity,
        Product product )
    {
        IsPickSlot = isPickSlot;
        Aisle = aisle;
        Bay = bay;
        Level = level;
        Length = length;
        Width = width;
        Height = height;
        Capacity = capacity;
        Product = product;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public bool IsPickSlot { get; private set; }
    public string Aisle { get; private set; }
    public string Bay { get; private set; }
    public string Level { get; private set; }
    public double Length { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double Capacity { get; private set; }
    public Guid OwnerId { get; private set; }
    public Employee? Owner { get; private set; }
    public Pallet? Pallet { get; private set; }
    public Product Product { get; private set; }

    public bool IsOwnedBy( Employee? employee ) =>
        Owner == employee;
    public bool IsAvailable() =>
        Owner is null &&
        Pallet is null;
    public bool PalletFits( Pallet pallet ) =>
        Length > pallet.Length &&
        Width > pallet.Width &&
        Height > pallet.Height;
    public bool AddPallet( Pallet pallet )
    {
        bool palletAdded = IsAvailable() 
            && PalletFits( pallet ) 
            && Capacity > pallet.Weight;
        
        if (palletAdded)
            Pallet = pallet;
        
        return palletAdded;
    }
    public bool ReplenPallet( Pallet pallet )
    {
        bool palletAdded = Pallet is null
            && PalletFits( pallet )
            && Capacity > pallet.Weight;

        if (palletAdded)
            Pallet = pallet;

        return palletAdded;
    }
    public bool RemovePallet()
    {
        Pallet = null;
        return true;
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