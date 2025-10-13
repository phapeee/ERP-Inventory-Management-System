using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PineConePro.Erp.Operation.Application.Commands.CreateMyAggregate;
using PineConePro.Erp.Operation.Domain.Interfaces;
using PineConePro.Erp.Operation.Infrastructure.Persistence.Repositories;

namespace PineConePro.Erp.Operation.Infrastructure.Extensions;

/// <summary>
/// Contains extension methods for setting up infrastructure services.
/// </summary>
public static class ModuleExtensions
{
    /// <summary>
    /// Adds infrastructure services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateMyAggregateCommand).Assembly));

        services.AddScoped<IMyAggregateRepository, MyAggregateRepository>();

        services.AddDbContext<Persistence.AppDbContext>(options =>
            options.UseNpgsql(configuration["DB_CONNECTION_STRING"]));

        return services;
    }
}