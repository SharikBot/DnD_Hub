using DnDCharacterManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DnDCharacterManager.Infrastructure.Data.Configurations;

public class RuleConfiguration : IEntityTypeConfiguration<Rule>
{
    public void Configure(EntityTypeBuilder<Rule> builder)
    {
        builder.ToTable("rules");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(16000);

        builder.Property(r => r.Category)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(r => r.Source)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(r => r.Category);
        builder.HasIndex(r => r.Title);
    }
}
