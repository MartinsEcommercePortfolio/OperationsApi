using OperationsDomain.Units;

namespace OperationsDomain.Outbound.Shipping.Models;

public sealed class ShippingOperations
{
    public Guid Id { get; private set; }
    public List<Trailer> Trailers { get; private set; } = [];
    public List<Dock> Docks { get; private set; } = [];
    public List<Trailer> InboundTrailers { get; private set; } = [];
    public List<Trailer> OutboundTrailers { get; private set; } = [];
    
    public Trailer? FindAvailableTrailer() =>
        Trailers.FirstOrDefault( static t => t.State is TrailerState.Parked );
    public Dock? FindAvailableDock() =>
        Docks.FirstOrDefault( static d => !d.IsOwned() );
}