// To use Entity Framework Core, add packages like:
// <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.*" />
// <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" /> // Or your preferred provider

using Microsoft.EntityFrameworkCore;
using PineConePro.Erp.PIM.Domain.Aggregates;
using System.Reflection;

namespace PineConePro.Erp.PIM.Infrastructure.Persistence;

internal class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<MyAggregate> MyAggregates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
