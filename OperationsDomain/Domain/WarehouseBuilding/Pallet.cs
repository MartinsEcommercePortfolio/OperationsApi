using OperationsDomain.Domain.Employees;

namespace OperationsDomain.Domain.WarehouseBuilding;

public sealed class Pallet
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid ReceivedById { get; set; }
    public Guid AreaId { get; set; }
    public Guid RackingId { get; set; }
    public Trailer? Trailer { get; set; }
    public Employee? Owner { get; set; }
    public Employee? ReceivedBy { get; set; }
    public Area? Area { get; set; }
    public Racking? Racking { get; set; }
    public List<Item> Items { get; set; } = [];
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }

    public bool IsEmpty() =>
        Items.Count <= 0;
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
    public bool CanBePickedUp() =>
        true;
    public bool CanBePickedFrom() =>
        true;

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
    public bool PlaceInArea( Area area )
    {
        if (Area is not null || Racking is not null || Owner is null)
            return false;
        
        ClearOwners();
        SetArea( area );
        return true;
    }
    public bool PutInRacking( Racking racking )
    {
        ClearOwners();
        SetRacking( racking );
        return true;
    }
    public bool LoadInTrailer( Trailer trailer )
    {
        if (!IsReceived() || Trailer is not null)
            return false;

        ClearOwners();
        Trailer = trailer;
        return true;
    }
    public bool PickItem( Employee employee, Guid itemId, out Item? item )
    {
        item = Items.FirstOrDefault( i => i.Id == itemId );
        if (item is null)
            return false;
        Items.Remove( item );
        item.GiveTo( employee );
        return true;
    }
    
    public bool GiveTo( Employee employee )
    {
        ClearOwners();
        SetOwner( employee );
        return true;
    }
    public void TakeFrom( Employee employee )
    {
        ClearOwners();
    }

    public static Pallet NewEmpty( Employee employee )
    {
        Pallet pallet = new();
        pallet.Id = Guid.NewGuid();
        pallet.GiveTo( employee );
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