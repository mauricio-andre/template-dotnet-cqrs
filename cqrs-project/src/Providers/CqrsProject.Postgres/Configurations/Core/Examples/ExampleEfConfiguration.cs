using CqrsProject.Core.Examples.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsProject.Postgres.Configurations.Core;

public class ExampleEfConfiguration : IEntityTypeConfiguration<Example>
{
    public void Configure(EntityTypeBuilder<Example> builder)
    {
        builder.ToTable("Examples");

        builder.HasKey(example => example.Id);

        builder.Property(example => example.Id)
            .ValueGeneratedOnAdd();

        builder.Property(example => example.Name);
    }
}
