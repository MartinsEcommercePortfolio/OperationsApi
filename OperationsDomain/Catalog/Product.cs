namespace OperationsDomain.Catalog;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int Stock { get; set; }
}