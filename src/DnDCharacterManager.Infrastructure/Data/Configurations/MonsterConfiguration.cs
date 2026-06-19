using DnDCharacterManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DnDCharacterManager.Infrastructure.Data.Configurations;

public class MonsterConfiguration : IEntityTypeConfiguration<Monster>
{
    public void Configure(EntityTypeBuilder<Monster> builder)
    {
        builder.ToTable("monsters");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.ChallengeRating)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(m => m.CreatureType)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(8000);

        builder.HasIndex(m => m.Name);
        builder.HasIndex(m => m.CreatureType);
    }
}
