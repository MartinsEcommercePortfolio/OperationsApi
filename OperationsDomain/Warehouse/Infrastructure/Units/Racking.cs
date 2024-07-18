using OperationsDomain.Catalog;
using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure.Units;

public sealed class Racking : PalletHolder
{
    public Racking(
        string aisle,
        string bay,
        string level,
        double length,
        double width,
        double height,
        double capacity,
        Product product ) 
        : base( 1 )
    {
        Id = new Guid();
        Aisle = aisle;
        Bay = bay;
        Level = level;
        Length = length;
        Width = width;
        Height = height;
        Capacity = capacity;
        Product = product;
    }
    
    public string Aisle { get; private set; }
    public string Bay { get; private set; }
    public string Level { get; private set; }
    public double Length { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double Capacity { get; private set; }
    public Pallet? Pallet { get; private set; }
    public Product Product { get; private set; }    
    
    public bool IsAvailable() =>
        Owner is null &&
        Pallet is null;
    public bool PalletFits( Pallet pallet ) =>
        Length > pallet.Length &&
        Width > pallet.Width &&
        Height > pallet.Height;
    public override bool AddPallet( Employee employee, Pallet pallet )
    {
        return PalletFits( pallet )
            && Capacity > pallet.Weight
            && base.AddPallet( employee, pallet );
    }
}