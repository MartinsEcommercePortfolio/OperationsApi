namespace OperationsDomain.Catalog;

public sealed class Product
{
    public Product( string title, string brand, int stockPallets = 0 )
    {
        Title = title;
        Brand = brand;
        PhysicalStockPallets = stockPallets;
        VirtualStockPallets = stockPallets;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
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