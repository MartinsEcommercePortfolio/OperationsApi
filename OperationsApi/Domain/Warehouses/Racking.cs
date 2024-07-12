namespace OperationsApi.Domain.Warehouses;

internal sealed class Racking
{
    public Guid Id { get; set; }
    public int Aisle { get; set; }
    public int Bay { get; set; }
    public int Level { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Capacity { get; set; }
}