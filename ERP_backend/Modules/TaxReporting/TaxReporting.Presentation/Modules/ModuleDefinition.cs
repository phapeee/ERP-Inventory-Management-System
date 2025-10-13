using Microsoft.AspNetCore.Routing;
using PineConePro.Erp.TaxReporting.Presentation.Extensions;

namespace PineConePro.Erp.TaxReporting.Presentation.Modules;

/// <summary>
/// Default module definition that wires controller-based endpoints.
/// </summary>
public sealed class ModuleDefinition : IModule
{
    /// <inheritdoc />
    public string Name => "TaxReporting";

    /// <inheritdoc />
    public string HealthEndpointUrl => "/api/my-aggregates/health";

    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapTaxReportingEndpoints();
    }
}
