using OperationsDomain.Units;

namespace OperationsDomain.Operations.Inbound.Models;

public sealed class InboundOperations
{
    public Guid Id { get; private set; }
    public List<Guid> AwaitingDock { get; private set; } = [];
    public List<Guid> AwaitingTask { get; private set; } = [];
    public List<Guid> Docked { get; private set; } = [];
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Dock> Docks { get; private set; } = [];

    public List<Trailer> GetTrailersAwaitingDock() =>
        Trailers.Where( t => AwaitingDock.Contains( t.Id ) ).ToList();
    public List<Trailer> GetTrailersAwaitingTask() =>
        Trailers.Where( t => AwaitingTask.Contains( t.Id ) ).ToList();
    public int? ReceiveTrailer( int trailerNumber )
    {
        if (Trailers.Any( t => t.Number == trailerNumber ))
            return null;

        // pallets are handled in sim and received one by one (trailer is black box for receiving)
        var trailer = Trailer.NewReceiving( trailerNumber );
        Trailers.Add( trailer );

        var dockReserved = ReserveDock( trailer, out var dock );

        if (dockReserved)
            return dock!.Number;
        
        AwaitingDock.Add( trailer.Id );

        return -1;
    }
    public bool ReserveDock( Trailer trailer, out Dock? dock )
    {
        dock = Docks.FirstOrDefault( static d =>
            !d.IsOwned() &&
            !d.TrailerNumber.HasValue &&
            d.Trailer is null );

        return dock is not null
            && dock.Reserve( trailer.Number );
    }
    public Trailer? DockTrailer( int trailerNumber, Guid dockId )
    {
        var trailer = Trailers.FirstOrDefault( t => t.Number == trailerNumber );
        var dock = Docks.FirstOrDefault( d => d.Id == dockId && d.TrailerNumber == trailerNumber );

        if (trailer is null || dock is null)
            return null;

        var docked = !Docked.Contains( trailer.Id )
            && dock.AssignTrailer( trailer )
            && trailer.DockTo( dock );

        if (!docked)
            return null;

        AwaitingTask.Add( trailer.Id );
        Docked.Add( trailer.Id );

        return trailer;
    }
    public bool UnDockTrailer( int trailerNumber, Guid dockId )
    {
        var trailer = Trailers.FirstOrDefault( t => t.Number == trailerNumber );
        var dock = trailer?.Dock;

        return trailer is not null
            && dock is not null
            && dock.Id != dockId
            && dock.UnAssignTrailer( trailer )
            && trailer.UnDock( dock )
            && Docked.Remove( trailer.Id )
            && Trailers.Remove( trailer );
    }
    public bool ConfirmTaskAssigned( Trailer trailer )
    {
        return !AwaitingDock.Contains( trailer.Id )
            && Docked.Contains( trailer.Id )
            && AwaitingTask.Remove( trailer.Id );
    }
}