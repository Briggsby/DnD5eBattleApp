using System.Collections.Generic;
using System.Linq;
using BugsbyEngine;
using Microsoft.Xna.Framework.Graphics;

namespace DnD5eBattleApp;

public class SpellBook
{
    public Creature Owner {get; set;}
    public CreatureValue SpellcastingAbility {get; set;}
    public Dictionary<int, List<Spell>> SpellsByLevel {get; set;}

    public List<Spell> AllSpells { get => SpellsByLevel.Values.ToList().SelectMany(x => x).ToList(); }

    public SpellBook(
        Creature owner,
        CreatureValue spellcastingAbility,
        List<string> spellSpecs
    )
    {
        Owner = owner;
        SpellcastingAbility = spellcastingAbility;
        SpellsByLevel = new Dictionary<int, List<Spell>>
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
        foreach (string spell in spellSpecs)
        {
            if (DnDManager.TryGetResource(spell, out SpellSpec spec))
            {
                SpellsByLevel[spec.Level].Add(spec.ToSpell(this));
            }
        }
    }

    public ContextMenuTemplate GetCommandMenu(
        ButtonTextures buttonTextures,
        SpriteFont spriteFont
    )
    {
        if (AllSpells.Count < 0)
        {
            return null;
        }

        List<ContextMenuTemplateItemDef> spellControlsEachLevel = new List<ContextMenuTemplateItemDef>();

        for (int level = 0; level <= 9; level++)
        {
            List<ContextMenuTemplateItemDef> spellItems = new List<ContextMenuTemplateItemDef>();
            for (int i = 0; i < SpellsByLevel[level].Count; i++)
            {
                var spell = SpellsByLevel[level][i];
                spellItems.Add(new ContextMenuTemplateItemDef(
                    text: spell.Name,
                    tags: new List<string>() {CombatActions.CastSpell.ToString(), level.ToString(), i.ToString() },
                    inactive: false // TODO: Check castable
                ));
            }
            if (spellItems.Count == 0)
            {
                continue;
            }

            spellControlsEachLevel.Add(new ContextMenuTemplateItemDef(
                text: level == 0 ? "Cantrips" : $"Level {level}",
                tags: new List<string>() { level.ToString(), ContextMenu.DefaultTags.ParentMenu.ToString()},
                inactive: false,
                childMenu: spellItems
            ));
        }

        return new ContextMenuTemplate(
            buttonTextures,
            spriteFont,
            spellControlsEachLevel
        );
    }
}