using OperationsDomain._Database;
using OperationsDomain.Operations.Inbound.Models;

namespace OperationsDomain.Operations.Inbound;

public interface IInboundRepository : IEfCoreRepository
{
    public Task<InboundOperations?> GetReceivingOperations();
}