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
    public SpellBook SpellBook {get; set;}
    public int SpellLevel {get; set;}
    public string SpellSchool {get; set;}
    public SpellComponent spellComponents {get; set;}

    public Spell(string name, Creature owner, Targeting targeting, ActionType actionType, Damage damage, SpellBook spellBook, int spellLevel, string spellSchool, SpellComponent spellComponents, string appliesCondition = null) : base(name, owner, targeting, actionType, damage, appliesCondition)
    {
        SpellBook = spellBook;
        SpellLevel = spellLevel;
        SpellSchool = spellSchool;
        this.spellComponents = spellComponents;
    }

    public override void SpendResources()
    {
        if (SpellLevel > 0)
        {
            Owner.spellSlots.spellSlotsCurrent[SpellLevel]--;
        }
        base.SpendResources();
    }

    public override int GetAttackBonus()
    {
        int bonus = 0;
        bonus += SpellBook.Owner.proficiencyBonus;
        bonus += SpellBook.Owner.StatMod(SpellBook.SpellcastingAbility);
        return bonus;
    }
}