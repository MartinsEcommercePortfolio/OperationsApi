using OperationsDomain.Catalog;
using OperationsDomain.Warehouse.Employees.Models;
using OperationsDomain.Warehouse.Operations.Picking.Models;
using OperationsDomain.Warehouse.Operations.Replenishing.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Pallet
{
    public Pallet( Guid id )
    {
        Id = id;
    }

    public static Pallet NewEmpty( Employee employee )
    {
        Pallet pallet = new( Guid.NewGuid() ) {
            Id = Guid.NewGuid()
        };
        pallet.AssignTo( employee );
        return pallet;
    }
    
    public Guid Id { get; private set; }
    public Guid? RackingId { get; private set; }
    public Guid? AreaId { get; private set; }
    public Guid? TrailerId { get; private set; }
    public Guid? EmployeeId { get; private set; }

    public Racking? Racking { get; private set; }
    public Area? Area { get; private set; }
    public Trailer? Trailer { get; private set; }
    public Employee? Employee { get; private set; }

    public Product Product { get; private set; }
    public int ItemCount { get; private set; }
    public List<Item> Items { get; private set; } = [];
    
    public double Length { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double Weight { get; private set; }

    public bool IsEmpty() =>
        Items.Count <= 0;
    public bool IsOwned() => 
        EmployeeId is not null;
    public bool IsStaged() =>
        AreaId is not null;

    public bool PickFrom( Racking racking, Employee employee, Guid itemId, out Item? item )
    {
        item = Items.FirstOrDefault( i => i.Id == itemId );
        
        return item is not null
            && racking == Racking
            && racking.IsOwnedBy( employee )
            && Items.Remove( item );
    }
    public bool AssignTo( Employee employee )
    {
        if (IsOwned())
            return false;
        
        ClearOwners();
        SetOwner( employee );
        return true;
    }
    
    void ClearOwners()
    {
        SetArea( null );
        SetOwner( null );
        SetRacking( null );
    }
    void SetOwner( Employee? employee )
    {
        Employee = employee;
        EmployeeId = employee?.Id ?? Guid.Empty;
    }
    void SetArea( Area? area )
    {
        Area = area;
        AreaId = area?.Id ?? Guid.Empty;
    }
    void SetRacking( Racking? racking )
    {
        Racking = racking;
        RackingId = racking?.Id ?? Guid.Empty;
    }
}