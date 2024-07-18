namespace OperationsDomain.Warehouse.Equipment;

public sealed class Scanner
{
    public Scanner( string number )
    {
        Id = Guid.NewGuid();
        Number = number;
    }
    
    public Guid Id { get; private set; }
    public string Number { get; private set; }
}