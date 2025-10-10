using Microsoft.AspNetCore.Routing;

namespace PineConePro.Erp.PIM.Presentation.Modules;

/// <summary>
/// Describes the contract for exposing module endpoints to the host application.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Gets the module identifier used for logging and diagnostics.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Maps the module's endpoints onto the provided route builder.
    /// </summary>
    /// <param name="endpoints">The route builder to populate.</param>
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}
