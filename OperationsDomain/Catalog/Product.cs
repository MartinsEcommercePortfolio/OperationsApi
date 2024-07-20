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
    public int PublicStockPallets { get; private set; }
    public int ReservedStockPallets { get; private set; }

    public bool InsertQuantity( int quantity )
    {
        PhysicalStockPallets += quantity;
        PublicStockPallets += quantity;
        return true;
    }
    public bool ReserveQuantity( int quantity )
    {
        if (PublicStockPallets <= quantity || PhysicalStockPallets <= quantity)
            return false;

        PublicStockPallets -= quantity;
        ReservedStockPallets += quantity;
        return true;
    }
    public bool Pick()
    {
        if (PhysicalStockPallets <= 0)
            return false;

        ReservedStockPallets--;
        PhysicalStockPallets--;
        return true;
    }
}