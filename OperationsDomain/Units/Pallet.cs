using OperationsDomain.Catalog;
using OperationsDomain.Employees.Models;
using OperationsDomain.Operations.Shipping.Models;

namespace OperationsDomain.Units;

public sealed class Pallet : Unit
{
    Pallet( Guid id, int number, Employee? employee, Product product, int itemCount )
        : base( id, number, employee )
    {
        Product = product;
        ItemCount = itemCount;
    }

    public static Pallet New( int number, Product product, int itemCount ) =>
        new( Guid.NewGuid(), number, null, product, itemCount );
    
    public Guid? RackingId { get; private set; }
    public Guid? AreaId { get; private set; }
    public Guid? TrailerId { get; private set; }
    
    public Racking? Racking { get; private set; }
    public Area? Area { get; private set; }
    public Trailer? Trailer { get; private set; }
    
    public Product Product { get; private set; }
    public int ItemCount { get; private set; }
    
    public double Length { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double Weight { get; private set; }

    public bool Pickup( Employee employee )
    {
        return IsFree()
            && IsOwnedBy( employee )
            && AssignTo( employee );
    }
    public bool SetTrailer( Trailer trailer )
    {
        if (!IsFree())
            return false;
        
        Trailer = trailer;
        TrailerId = trailer.Id;
        return true;
    }
    public bool SetArea( Area area )
    {
        if (!IsFree())
            return false;

        Area = area;
        AreaId = area.Id;
        return true;
    }
    public bool SetRacking( Racking racking )
    {
        if (!IsFree())
            return false;

        Racking = racking;
        RackingId = racking.Id;
        return true;
    }

    bool IsFree() =>
        Racking is null &&
        Area is null &&
        Trailer is null;
}