// Data/AppDb.cs
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace ErpApi.Data;

[SuppressMessage("Design", "CA1515:Consider marking type as internal", Justification = "DbContext must be public to satisfy controller constructor accessibility.")]
public class AppDb(DbContextOptions<AppDb> options) : DbContext(options)
{
    internal DbSet<Product> Products => Set<Product>();
}

internal sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}
