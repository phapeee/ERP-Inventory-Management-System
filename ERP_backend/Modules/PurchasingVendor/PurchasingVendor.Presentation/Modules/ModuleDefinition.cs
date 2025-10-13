using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.PurchasingVendor.Presentation.Extensions;

namespace PineConePro.Erp.PurchasingVendor.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "PurchasingVendor";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPurchasingVendorEndpoints();
    }
}
