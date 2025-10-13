using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.Accounting.Presentation.Extensions;

namespace PineConePro.Erp.Accounting.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "Accounting";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAccountingEndpoints();
    }
}
