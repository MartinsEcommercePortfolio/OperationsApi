namespace OperationsDomain.Catalog;

public sealed class Product
{
    public Product( string title, string brand, int stock = 0 )
    {
        Title = title;
        Brand = brand;
        PhysicalStock = stock;
        VirtualStock = stock;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; }
    public string Brand { get; private set; }
    public int PhysicalStock { get; private set; }
    public int VirtualStock { get; private set; }

    public bool ReservePickAmount( int quantity )
    {
        if (VirtualStock <= quantity || PhysicalStock <= quantity)
            return false;

        VirtualStock -= quantity;
        return true;
    }
}