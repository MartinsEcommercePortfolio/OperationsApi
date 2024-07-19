namespace OperationsDomain.Warehouse.Equipment;

public sealed class Forklift
{
    Forklift( Guid id, string number )
    {
        Id = id;
        Number = number;
    }

    public static Forklift New( string number ) =>
        new( Guid.NewGuid(), number );
    
    public Guid Id { get; private set; }
    public string Number { get; private set; }
}