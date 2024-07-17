namespace OperationsDomain.Shipping;

public sealed class Route
{
    public Route()
    {
        Id = Guid.NewGuid();
        Addresses = [];
    }
    
    public Guid Id { get; private set; }
    public List<Address> Addresses { get; private set; }

    public bool ContainsAddress( int posX, int posY )
    {
        return Addresses.Any( a =>
            a.PosX == posX && a.PosY == posY );
    }
}