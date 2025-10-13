using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.Shipping.Presentation.Extensions;

namespace PineConePro.Erp.Shipping.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "Shipping";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapShippingEndpoints();
    }
}
