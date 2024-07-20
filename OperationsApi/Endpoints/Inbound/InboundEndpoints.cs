using Microsoft.AspNetCore.Mvc;
using OperationsDomain.Operations.Inbound;
using OperationsDomain.Operations.Inbound.Models;

namespace OperationsApi.Endpoints.Inbound;

internal static class InboundEndpoints
{
    internal static void MapInboundEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapPost( "api/inbound/receiveTrailer",
            static async (
                    [FromQuery] int trailerNumber,
                    IInboundRepository receivingRepository ) =>
                await ReceiveTrailer( 
                    trailerNumber, 
                    receivingRepository ) );

        app.MapPost( "api/inbound/dockTrailer",
            static async (
                    [FromQuery] int trailerNumber,
                    [FromQuery] Guid dockId,
                    IInboundRepository receivingRepository ) =>
                await DockTrailer(
                    trailerNumber,
                    dockId,
                    receivingRepository ) );

        app.MapPost( "api/inbound/unDockTrailer",
            static async (
                    [FromQuery] int trailerNumber,
                    [FromQuery] Guid dockId,
                    IInboundRepository receivingRepository ) =>
                await UnDockTrailer(
                    trailerNumber,
                    dockId,
                    receivingRepository ) );
    }

    static async Task<IResult> ReceiveTrailer( int trailerNumber, IInboundRepository inboundRepository )
    {
        InboundOperations? receiving = await inboundRepository.GetInboundOperations();
        if (receiving is null)
            return Results.NotFound();
        
        // -1 if received but no dock available
        var dockNumber = receiving.ReceiveTrailer( trailerNumber );

        return dockNumber is not null && await inboundRepository.SaveAsync()
            ? Results.Ok( dockNumber )
            : Results.Problem();
    }
    static async Task<IResult> DockTrailer( int trailerNumber, Guid dockId, IInboundRepository inboundRepository )
    {
        var receiving = await inboundRepository.GetInboundOperations();
        if (receiving is null)
            return Results.NotFound();

        var trailer = receiving.DockTrailer( trailerNumber, dockId );

        return trailer is not null && await inboundRepository.SaveAsync()
            ? Results.Ok( trailer.Dock!.Number )
            : Results.Problem();
    }
    static async Task<IResult> UnDockTrailer( int trailerNumber, Guid dockId, IInboundRepository inboundRepository )
    {
        var receiving = await inboundRepository.GetInboundOperations();
        if (receiving is null)
            return Results.NotFound();

        return receiving.UnDockTrailer( trailerNumber, dockId ) && await inboundRepository.SaveAsync()
            ? Results.Ok()
            : Results.Problem();
    }
}