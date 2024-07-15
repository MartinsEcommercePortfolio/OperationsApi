using OperationsDomain.Warehouses.Employees;

namespace OperationsDomain.Warehouses.Infrastructure;

public sealed class Item
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Employee? Owner { get; set; }

    public bool CanBePicked() =>
        true;

    public bool GiveTo( Employee employee )
    {
        if (Owner is not null)
            return false;

        Owner = employee;
        return true;
    }
}