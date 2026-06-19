using DnDCharacterManager.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DnDCharacterManager.Infrastructure.Data.Configurations;

public class CharacterTraitConfiguration : IEntityTypeConfiguration<CharacterTrait>
{
    public void Configure(EntityTypeBuilder<CharacterTrait> builder)
    {
        builder.ToTable("character_traits");
        builder.HasKey(ct => new { ct.CharacterId, ct.TraitId });
    }
}

public class CharacterSkillConfiguration : IEntityTypeConfiguration<CharacterSkill>
{
    public void Configure(EntityTypeBuilder<CharacterSkill> builder)
    {
        builder.ToTable("character_skills");
        builder.HasKey(cs => new { cs.CharacterId, cs.SkillId });
    }
}

public class CharacterSpellConfiguration : IEntityTypeConfiguration<CharacterSpell>
{
    public void Configure(EntityTypeBuilder<CharacterSpell> builder)
    {
        builder.ToTable("character_spells");
        builder.HasKey(csp => new { csp.CharacterId, csp.SpellId });
    }
}
