using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace PineConePro.Erp.PIM.Presentation.Extensions;

/// <summary>
/// Contains extension methods for setting up presentation services.
/// </summary>
public static class ModuleExtensions
{
    /// <summary>
    /// Maps the endpoints for the module.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to add endpoints to.</param>
    /// <returns>The <see cref="IEndpointRouteBuilder"/> so that additional calls can be chained.</returns>
    public static IEndpointRouteBuilder MapPIMEndpoints(this IEndpointRouteBuilder app)
    {
        // This is the single entry point for registering all endpoints in the module.
        app.MapControllers();
        // TODO: Apply additional conventions or endpoint-specific configuration here.

        return app;
    }
}
