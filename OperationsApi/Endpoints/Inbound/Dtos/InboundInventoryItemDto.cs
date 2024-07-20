using OperationsDomain.Catalog;
using OperationsDomain.Units;

namespace OperationsApi.Endpoints.Inbound.Dtos;

internal readonly record struct InboundInventoryItemDto(
    int ItemNumber,
    Guid ProductId,
    int Quantity )
{
    public Pallet ToPallet( Product product ) =>
        Pallet.New( ItemNumber, product, Quantity );
}