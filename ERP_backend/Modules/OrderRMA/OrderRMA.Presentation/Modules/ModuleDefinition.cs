using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.OrderRMA.Presentation.Extensions;

namespace PineConePro.Erp.OrderRMA.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "OrderRMA";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapOrderRMAEndpoints();
    }
}
