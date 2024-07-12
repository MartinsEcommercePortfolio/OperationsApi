namespace OperationsApi.Features;

internal static class Endpoints
{
    internal static void MapEndpoints( this IEndpointRouteBuilder app )
    {
        app.MapGet( "api/inventory", static async () => { } );
        
        // EMPLOYEE
        // - sign in
        // - sign out
        // - start break
        // - end break
        // - log forklift
        // - logout forklift
        // - log scanner
        // - logout scanner
        // - get next task
        // - start task
        // - get next sub-task
        // - start sub-task
        // - confirm sub-task
        // - start complete
        // - confirm complete
        // - request replen
        // - start forklift recharge
        // - stop forklift recharge
        // - start forklift repair
        // - complete forklift repair
        // - update employee state
        // - quit job (if employees unhappy from short breaks, they may quit or steal)
        // - steal item
        // - update forklift status
        // - update scanner status
        // - employee wait (for example no forklifts or scanners available)
        // - receive shipment
    }
}