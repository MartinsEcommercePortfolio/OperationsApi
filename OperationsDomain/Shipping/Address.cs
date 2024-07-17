namespace OperationsDomain.Shipping;

public sealed class Address
{
    public Address( string name, int x, int y )
    {
        Name = name;
        PosX = x;
        PosY = y;
    }
    
    public string Name { get; private set; }
    public int PosX { get; private set; }
    public int PosY { get; private set; }
}