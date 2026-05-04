using System;
using DnD5eBattleApp;

[Flags]
public enum SpellComponent
{
    None = 0,
    Verbal = 1,
    Somatic = 2,
    Material = 4
}

public class Spell : Ability
{
    public int SpellLevel {get; set;}
    public string SpellSchool {get; set;}
    public SpellComponent spellComponents {get; set;}
    public Spell(SpellSpec spellSpec) : base(spellSpec)
    {
        SpellLevel = spellSpec.Level;
        SpellSchool = spellSpec.School;
    }

    public override void SpendResources(Creature user)
    {
        if (SpellLevel > 0)
        {
            user.spellSlots.spellSlotsCurrent[SpellLevel]--;
        }
        base.SpendResources(user);
    }
}