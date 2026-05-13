using System;
using System.Collections.Generic;

namespace DnD5eBattleApp;

public record SpellSpec : AbilitySpec
{
    public required int Level {get; init;}
    public required string School {get; init;}
    public required SpellComponent Components {get; init;}

    public Spell ToSpell(SpellBook spellBook)
    {
        return new Spell(Name, Targeting, ActionType, Damage, spellBook, Level, School, Components, AppliesCondition);
    }
}