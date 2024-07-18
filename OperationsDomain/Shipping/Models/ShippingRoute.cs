namespace OperationsDomain.Shipping.Models;

public sealed class ShippingRoute
{
    public ShippingRoute()
    {
        Id = Guid.NewGuid();
        Addresses = [];
    }
    
    public Guid Id { get; private set; }
    public List<ShippingAddress> Addresses { get; private set; }

    public bool ContainsAddress( int posX, int posY )
    {
        return Addresses.Any( a =>
            a.PosX == posX && a.PosY == posY );
    }
}