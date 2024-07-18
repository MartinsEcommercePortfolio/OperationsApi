namespace OperationsDomain.Warehouse.Infrastructure.Units;

public sealed class Dock : InfrastructureUnit
{
    internal Dock( string number )
    {
        Id = Guid.NewGuid();
        Number = number;
    }

    public static Dock Create( string number ) =>
        new( number );
    
    public string Number { get; private set; }
    public Trailer? Trailer { get; private set; }
}