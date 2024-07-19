namespace OperationsDomain.Catalog;

public sealed class Product
{
    Product( Guid id, string title, string brand )
    {
        Id = id;
        Title = title;
        Brand = brand;
    }

    public static Product New( string title, string brand ) =>
        new( Guid.NewGuid(), title, brand );

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Brand { get; private set; }
    public int PhysicalStockPallets { get; private set; }
    public int VirtualStockPallets { get; private set; }
    public int ReservedStockPallets { get; private set; }

    public bool InsertQuantity( int quantity )
    {
        PhysicalStockPallets += quantity;
        VirtualStockPallets += quantity;
        return true;
    }
    public bool ReserveQuantity( int quantity )
    {
        if (VirtualStockPallets <= quantity || PhysicalStockPallets <= quantity)
            return false;

        VirtualStockPallets -= quantity;
        ReservedStockPallets += quantity;
        return true;
    }
    public bool PickQuantity( int quantity )
    {
        if (PhysicalStockPallets < quantity)
            return false;

        ReservedStockPallets -= quantity;
        PhysicalStockPallets -= quantity;
        return true;
    }
}