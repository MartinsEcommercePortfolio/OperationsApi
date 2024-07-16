using OperationsDomain.Domain.Catalog;
using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Pallet
{
    public Pallet( Guid id )
    {
        Id = id;
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
    
    public bool IsOwned() => 
        Employee is not null;
    public bool IsOwnedBy( Employee employee ) =>
        Employee == employee;
    public bool IsRacked() =>
        RackingId is not null || Racking is not null;
    public bool IsStaged() =>
        AreaId is not null || Area is not null;
    public bool IsCorrectArea( Guid areaId ) =>
        AreaId == areaId;

    public bool CanBeStaged( Employee employee ) =>
        IsOwnedBy( employee ) &&
        Area is not null &&
        Racking is not null;
    public bool CanBeRacked( Employee employee ) =>
        IsOwnedBy( employee );
    public bool CanBePutaway() =>
        !IsOwned() &&
        IsStaged();
    public bool CanBePicked() =>
        !IsOwned() &&
        IsRacked();
    public bool CanBePickedFrom() =>
        !IsOwned() &&
        IsRacked();
    
    public bool Stage( Employee employee, Area area )
    {
        if (!CanBeStaged( employee ))
            return false;
        
        ClearOwners();
        SetArea( area );
        return true;
    }
    public bool Pick( Employee employee )
    {
        return false;
    }
    public bool Rack( Employee employee, Racking racking )
    {
        if (!CanBeRacked( employee ))
            return false;
        
        ClearOwners();
        SetRacking( racking );
        return true;
    }
    public bool Load( Trailer trailer )
    {
        if (Trailer is not null)
            return false;

        ClearOwners();
        Trailer = trailer;
        return true;
    }
    public bool PickFrom( Racking racking, Employee employee, Guid itemId, out Item? item )
    {
        item = Items.FirstOrDefault( i => i.Id == itemId );
        return racking == Racking
            && racking.IsOwnedBy( employee )
            && item is not null
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

    public static Pallet NewEmpty( Employee employee )
    {
        Pallet pallet = new(Guid.NewGuid());
        pallet.Id = Guid.NewGuid();
        pallet.AssignTo( employee );
        return pallet;
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