using OperationsDomain.Units;

namespace OperationsDomain.Operations.Inbound.Models;

public sealed class InboundOperations
{
    public Guid Id { get; private set; }
    public List<int> NewTrailers { get; private set; } = [];
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Dock> Docks { get; private set; } = [];
    public List<Area> Areas { get; private set; } = [];

    public int? ReceiveTrailer( int trailerNumber )
    {
        var existing = NewTrailers.Any( t => t == trailerNumber )
            || Trailers.Any( t => t.Number == trailerNumber );
        
        if (existing)
            return null;

        NewTrailers.Add( trailerNumber );
        
        var dock = Docks.FirstOrDefault( static d =>
            !d.IsOwned() &&
            !d.TrailerNumber.HasValue &&
            d.Trailer is null );

        var reserved = dock is not null
            && dock.Reserve( trailerNumber );

        return reserved
            ? dock!.Number
            : null;
    }
    public Trailer? DockTrailer( int trailerNumber, Guid dockId )
    {
        var validNumber = Trailers.All( t => t.Number != trailerNumber ) 
            && NewTrailers.Remove( trailerNumber );

        if (!validNumber)
            return null;

        var dock = Docks.FirstOrDefault( d =>
            d.Id == dockId && d.TrailerNumber == trailerNumber );

        if (dock is null)
            return null;

        var trailer = Trailer.New( trailerNumber );
        Trailers.Add( trailer );

        return dock.AssignTrailer( trailer )
            && trailer.DockTo( dock )
                ? trailer
                : null;
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
            && Trailers.Remove( trailer );
    }
}