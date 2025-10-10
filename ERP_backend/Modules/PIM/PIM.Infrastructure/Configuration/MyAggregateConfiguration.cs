using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PIM.Domain.Aggregates;

namespace PIM.Infrastructure.Configuration;

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
