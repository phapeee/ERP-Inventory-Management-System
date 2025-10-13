using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PineConePro.Erp.TaxReporting.Domain.Aggregates;

namespace PineConePro.Erp.TaxReporting.Infrastructure.Configuration;

internal class MyAggregateConfiguration : IEntityTypeConfiguration<MyAggregate>
{
    public void Configure(EntityTypeBuilder<MyAggregate> builder)
    {
        builder.ToTable("MyAggregates");

        builder.HasKey(a => a.Id);

        // Example of configuring a property
        // builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
    }
}
