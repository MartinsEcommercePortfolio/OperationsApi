using OperationsApi.Types.Warehouses;

namespace OperationsApi.Domain.Shipping;

internal sealed class Load
{
    public Guid Id { get; set; }
    public List<Pallet> Pallets { get; set; } = [];
}