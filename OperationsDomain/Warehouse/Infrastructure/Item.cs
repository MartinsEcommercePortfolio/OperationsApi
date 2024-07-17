using OperationsDomain.Warehouse.Employees.Models;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Item
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Employee? Owner { get; private set; }
    public double Length { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double Weight { get; private set; }
    

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