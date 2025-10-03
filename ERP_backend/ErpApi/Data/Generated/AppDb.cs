using System;
using System.Collections.Generic;
using ErpApi.Data.Generated.Entities;
using Microsoft.EntityFrameworkCore;

namespace ErpApi.Data.Generated;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider marking type as internal", Justification = "Generated context must remain public for DI.")]
public partial class AppDb : DbContext
{
    public AppDb()
    {
    }

    public AppDb(DbContextOptions<AppDb> options)
        : base(options)
    {
    }

    internal virtual DbSet<Product> Products { get; set; } = null!;

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
