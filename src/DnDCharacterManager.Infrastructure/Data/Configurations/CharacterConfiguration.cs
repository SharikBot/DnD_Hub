using DnDCharacterManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DnDCharacterManager.Infrastructure.Data.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("characters");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Backstory)
            .HasMaxLength(4000);

        builder.Property(c => c.PortraitPath)
            .HasMaxLength(500);

        builder.Property(c => c.Alignment)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Characters)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Race)
            .WithMany(r => r.Characters)
            .HasForeignKey(c => c.RaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CharacterClass)
            .WithMany(cc => cc.Characters)
            .HasForeignKey(c => c.CharacterClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Background)
            .WithMany(b => b.Characters)
            .HasForeignKey(c => c.BackgroundId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Inventory)
            .WithOne(i => i.Character)
            .HasForeignKey<Inventory>(i => i.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.CharacterTraits)
            .WithOne(ct => ct.Character)
            .HasForeignKey(ct => ct.CharacterId);

        builder.HasMany(c => c.CharacterSkills)
            .WithOne(cs => cs.Character)
            .HasForeignKey(cs => cs.CharacterId);

        builder.HasMany(c => c.CharacterSpells)
            .WithOne(csp => csp.Character)
            .HasForeignKey(csp => csp.CharacterId);

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.Name);
    }
}
