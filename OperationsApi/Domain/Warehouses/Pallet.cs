using OperationsApi.Domain.Employees;
using OperationsApi.Domain.Shipping;

namespace OperationsApi.Domain.Warehouses;

internal sealed class Pallet
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ReceivedById { get; set; }
    public Guid AreaId { get; set; }
    public Guid RackingId { get; set; }
    public Employee? Owner { get; set; }
    public Employee? ReceivedBy { get; set; }
    public Area? Area { get; set; }
    public Racking? Racking { get; set; }
    public List<Item> Items { get; set; } = [];
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    
    public bool IsOwned() => 
        Owner is not null;
    public bool IsOwnedBy( Employee employee ) =>
        Owner == employee;
    public bool IsReceived() => 
        ReceivedBy is not null;
    public bool IsStaged() =>
        Area is not null;
    public bool IsCorrectArea( Guid areaId ) =>
        AreaId == areaId;

    public bool CanBePutAway() =>
        IsReceived() &&
        !IsOwned() &&
        IsStaged();

    public bool ReceiveBy( Employee employee )
    {
        if (IsReceived())
            return false;
        
        ClearOwners();
        SetOwner( employee );
        ReceivedBy = employee;
        ReceivedById = employee.Id;

        return true;
    }
    public bool PutIn( Area area )
    {
        if (Area is not null || Racking is not null || Owner is null)
            return false;
        
        ClearOwners();
        SetArea( area );
        return true;
    }
    public void PutAway( Racking racking )
    {
        ClearOwners();
        SetRacking( racking );
    }
    public bool Load( Trailer trailer )
    {
        if (!IsReceived())
            return false;
        
        ClearOwners();
        return true;
    }
    public Item? Pick( Guid itemId )
    {
        Item? item = Items.FirstOrDefault( i => i.Id == itemId );
        if (item is not null)
            Items.Remove( item );
        return item;
    }
    
    public void AssignTo( Employee employee )
    {
        ClearOwners();
        SetOwner( employee );
    }
    public void UnassignFrom( Employee employee )
    {
        ClearOwners();
    }
    public void AssignTo( Racking racking )
    {
        SetOwner( null );
        RackingId = racking.Id;
        Racking = racking;
    }

    void ClearOwners()
    {
        SetArea( null );
        SetOwner( null );
        SetRacking( null );
    }
    
    void SetOwner( Employee? employee )
    {
        Owner = employee;
        OwnerId = employee?.Id ?? Guid.Empty;
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