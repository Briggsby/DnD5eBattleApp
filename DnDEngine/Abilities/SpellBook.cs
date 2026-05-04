using System.Collections.Generic;
using System.Linq;

namespace DnD5eBattleApp;

public class SpellBook
{
    public string SpellcastingAbility {get; set;}
    public Dictionary<int, List<Spell>> SpellsByLevel {get; set;}

    public List<Spell> AllSpells { get => SpellsByLevel.Values.ToList().SelectMany(x => x).ToList(); }

    public SpellBook(
        string spellcastingAbility,
        List<SpellSpec> spellSpecs
    )
    {
        SpellcastingAbility = spellcastingAbility;
        Dictionary<int, List<Spell>> SpellsByLevel = new Dictionary<int, List<Spell>>
        {
            {0, new List<Spell>() },
            {1, new List<Spell>() },
            {2, new List<Spell>() },
            {3, new List<Spell>() },
            {4, new List<Spell>() },
            {5, new List<Spell>() },
            {6, new List<Spell>() },
            {7, new List<Spell>() },
            {8, new List<Spell>() },
            {9, new List<Spell>() }
        };
        foreach (SpellSpec spell in spellSpecs)
        {
            SpellsByLevel[spell.Level].Add(new Spell(spell));
        }
    }

    public Spell AddSpell(SpellSpec spec)
    {
        Spell spell = new Spell(spec);
        SpellsByLevel[spec.Level].Add(spell);
        return spell;
    }
}