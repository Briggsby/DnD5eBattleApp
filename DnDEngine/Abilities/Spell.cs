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

    public Spell(string name, Targeting targeting, ActionType actionType, Damage damage, SpellBook spellBook, int spellLevel, string spellSchool, SpellComponent spellComponents, string appliesCondition = null) : base(name, targeting, actionType, damage, appliesCondition)
    {
        SpellBook = spellBook;
        SpellLevel = spellLevel;
        SpellSchool = spellSchool;
        this.spellComponents = spellComponents;
    }

    public override void SpendResources(Creature user)
    {
        if (SpellLevel > 0)
        {
            user.spellSlots.spellSlotsCurrent[SpellLevel]--;
        }
        base.SpendResources(user);
    }

    public override int GetAttackBonus(Creature user)
    {
        int bonus = 0;
        bonus += user.proficiencyBonus;
        bonus += user.StatMod(SpellBook.SpellcastingAbility);
        return bonus;
    }
}