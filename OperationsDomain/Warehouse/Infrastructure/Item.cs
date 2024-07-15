using OperationsDomain.Warehouse.Employees;

namespace OperationsDomain.Warehouse.Infrastructure;

public sealed class Item
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Employee? Owner { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    

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