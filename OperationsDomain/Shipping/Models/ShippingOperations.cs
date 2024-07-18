using OperationsDomain.Warehouse.Infrastructure.Units;

namespace OperationsDomain.Shipping.Models;

public sealed class ShippingOperations
{
    public Guid Id { get; private set; }
    public List<Trailer> Trailers { get; private set; }
}