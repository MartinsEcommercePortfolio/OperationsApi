namespace OperationsDomain.Warehouse.Equipment;

public sealed class Scanner
{
    Scanner( Guid id, string number )
    {
        Id = id;
        Number = number;
    }

    public static Scanner New( string number ) =>
        new( Guid.NewGuid(), number );
    
    public Guid Id { get; private set; }
    public string Number { get; private set; }
}