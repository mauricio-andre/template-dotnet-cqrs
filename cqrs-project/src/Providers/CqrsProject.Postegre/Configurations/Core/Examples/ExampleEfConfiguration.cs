using CqrsProject.Core.Examples;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CqrsProject.Postegre.Configurations;

public class ExampleEfConfiguration : IEntityTypeConfiguration<Example>
{
    public void Configure(EntityTypeBuilder<Example> builder)
    {
        builder.ToTable("Examples");

        builder.HasKey(example => example.Id);

        builder.Property(example => example.Id)
            .ValueGeneratedOnAdd();

        builder.Property(example => example.Name)
            .IsRequired()
            .HasMaxLength(ExampleConstrains.NameMaxLength);
    }
}
