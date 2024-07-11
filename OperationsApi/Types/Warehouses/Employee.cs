namespace OperationsApi.Types.Warehouses;

internal sealed class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

internal enum EmployeeType
{
    Picking,
    Receiving,
    Putaways,
    Replens,
    Loading
}