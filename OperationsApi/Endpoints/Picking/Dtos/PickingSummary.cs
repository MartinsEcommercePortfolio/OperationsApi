using OperationsApi.Endpoints.Dtos;
using OperationsDomain.Units;

namespace OperationsApi.Endpoints.Picking.Dtos;

internal readonly record struct PickingSummary(
    PalletDto Pallet,
    RackingDto Racking )
{
    internal static PickingSummary FromModel( Pallet pallet, Racking racking ) => new(
        PalletDto.FromModel( pallet ),
        RackingDto.FromModel( racking ) );
}