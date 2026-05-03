using System;
using System.Collections.Generic;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class SpellBook
    {
        public Stat abilityScore;
        public static ContextMenuTextureSet baseContextMenuTextureSet;
        public ContextMenuTextureSet contextMenuTextures;
        public List<Spell> spells;
        public Dictionary<int, List<Spell>> spellsByLevel;
        public Creature owner;

        public SpellBook()
        {
            spells = new List<Spell>();
            spellsByLevel = BlankSpellsByLevel();
            contextMenuTextures = baseContextMenuTextureSet;
        }

        public SpellBook(Creature creature)
        {
            spells = new List<Spell>();
            spellsByLevel = BlankSpellsByLevel();
            contextMenuTextures = baseContextMenuTextureSet;
            owner = creature;
        }

        public SpellBook(List<Spell> spells)
        {
            if (spells == null)
            {
                spells = new List<Spell>();
                spellsByLevel = BlankSpellsByLevel();
            }
            this.spells = new List<Spell>(spells);
            spellsByLevel = SortSpellsByLevel(spells);
            contextMenuTextures = baseContextMenuTextureSet;
        }

        public SpellBook(SpellBookSpec spellBookSpec, Creature owner)
        {
            this.owner = owner;
            spells = new List<Spell>();
            foreach (string spellName in spellBookSpec.Spells)
            {
                if (DnDManager.spells.ContainsKey(spellName))
                {
                    spells.Add(new Spell(DnDManager.spells[spellName], owner));
                }
                else
                {
                    Console.WriteLine(string.Format("Spell {0} not found in DnDManager.spells", spellName));
                }
            }
            spellsByLevel = SortSpellsByLevel(spells);
            contextMenuTextures = baseContextMenuTextureSet;
        }

        public SpellBook(List<Spell> spells, Creature creature, Stat spellcastingStat = Stat.None)
        {
            contextMenuTextures = baseContextMenuTextureSet;
            if (spells == null)
            {
                spells = new List<Spell>();
            }
            this.spells = new List<Spell>(spells);
            spellsByLevel = SortSpellsByLevel();
            AssignOwner(creature);

            if (spellcastingStat != Stat.None)
            {
                foreach( Spell spell in spells)
                {
                    spell.abilityModifier = spellcastingStat;
                }
            }
        }

        public SpellBook(List<Spell> spells, Dictionary<int, List<Spell>> spellsByLevel, ContextMenuTextureSet contextMenuTextures)
        {
            this.spells = new List<Spell>(spells);
            this.spellsByLevel = new Dictionary<int, List<Spell>>(spellsByLevel);
            this.contextMenuTextures = contextMenuTextures;
        }

        public SpellBook(SpellBook copy)
        {
            spells = new List<Spell>(copy.spells);
            spellsByLevel = new Dictionary<int, List<Spell>>(copy.spellsByLevel);
            contextMenuTextures = copy.contextMenuTextures;
        }

        public Dictionary<int, List<Spell>> BlankSpellsByLevel()
        {
            return new Dictionary<int, List<Spell>>()
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
                {9, new List<Spell>() },
            };
        }

        public static Dictionary<int, List<Spell>> SortSpellsByLevelStatic(List<Spell> spells)
        {
            Dictionary<int, List<Spell>> spellsSortedByLevel = new Dictionary<int, List<Spell>>
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
            foreach (Spell spell in spells)
            {
                spellsSortedByLevel[spell.spellLevel].Add(spell);
            }
            return spellsSortedByLevel;
        }

        public Dictionary<int, List<Spell>> SortSpellsByLevel(List<Spell> spells = null)
        {
            List<Spell> spellsToSort;
            if (spells != null)
            {
                spellsToSort = new List<Spell>(spells);
            }
            else
            {
                spellsToSort = new List<Spell>(this.spells);
            }
            Dictionary<int,List<Spell>> spellsSortedByLevel = new Dictionary<int, List<Spell>>
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
            foreach (Spell spell in spellsToSort)
            {
                spellsSortedByLevel[spell.spellLevel].Add(spell);
            }
            if (spells == null)
            {
                spellsByLevel = spellsSortedByLevel;
            }
            return spellsSortedByLevel;
        }

        public SpellBook CopyNewSpellBook(bool alsoTextures = false)
        {
            return new SpellBook(spells);
        }

        public ContextMenuTemplate SpellControls()
        {
            if (!(spells.Count>0))
            {
                return null;
            }
            ContextMenuTemplate spellControls = new ContextMenuTemplate();
            ContextMenuTemplate[] spellControlsEachLevel = new ContextMenuTemplate[10];

            for (int i = 0; i <= 9; i++)
            {
                spellControlsEachLevel[i] = new ContextMenuTemplate();
                spellControlsEachLevel[i].textures = ButtonTextures.FromList(contextMenuTextures.baseTextures);
                spellControlsEachLevel[i].font = contextMenuTextures.baseFont;
                spellControlsEachLevel[i].texts = new List<string>();
                spellControlsEachLevel[i].tags = new List<List<string>>();
                spellControlsEachLevel[i].inactives = new List<bool>();
                spellControlsEachLevel[i].childMenus = new List<ContextMenuTemplate>();
            }

            foreach (Spell spell in spells)
            {
                if (!spell.hideInSpellbook)
                {
                    spellControlsEachLevel[spell.spellLevel].texts.Add(spell.name);
                    if (spell.childMenu)
                    {
                        spellControlsEachLevel[spell.spellLevel].tags.Add(new List<string>() { ContextMenu.DefaultTags.ParentMenu.ToString() });
                    }
                    else
                    {
                        spellControlsEachLevel[spell.spellLevel].tags.Add(new List<string>() { CombatActions.CastSpell.ToString(), spells.IndexOf(spell).ToString() });
                    }
                    spellControlsEachLevel[spell.spellLevel].inactives.Add(!spell.CheckCastable(owner));
                    spellControlsEachLevel[spell.spellLevel].childMenus.Add(spell.GetChildMenu());

                }
            }

            spellControls.textures = ButtonTextures.FromList(contextMenuTextures.baseTextures);
            spellControls.font = contextMenuTextures.baseFont;
            spellControls.texts = new List<string>();
            spellControls.tags = new List<List<string>>();
            spellControls.childMenus = new List<ContextMenuTemplate>();
            if (spellsByLevel[0].Count > 0)
            {
                spellControls.texts.Add("Cantrips");
                spellControls.tags.Add(new List<string>() { 0.ToString(), ContextMenu.DefaultTags.ParentMenu.ToString() });
                spellControls.childMenus.Add(spellControlsEachLevel[0]);
            }
            for (int i = 1; i<=9; i++)
            {
                if (ContainsSpellLevel(i))
                {
                    spellControls.texts.Add(string.Format("Level {0}", i));
                    spellControls.tags.Add(new List<string>() { i.ToString(), ContextMenu.DefaultTags.ParentMenu.ToString() });
                    spellControls.childMenus.Add(spellControlsEachLevel[i]);
                }
            }

            return spellControls;

        }

        public bool ContainsSpellLevel(int i, List<Spell> spellList = null)
        {
            Dictionary<int, List<Spell>> spellsSortedByLevel;
            if (spellList == null)
            {
                spellsSortedByLevel = spellsByLevel;
            }
            else
            {
                spellsSortedByLevel = SortSpellsByLevel(spellList);
            }

            return (spellsSortedByLevel[i].Count > 0);
        }

        public void AssignOwner(Creature creature)
        {
            owner = creature;
            foreach(Spell spell in spells)
            {
                spell.caster = creature;
                spell.spellbook = this;
            }
        }

        public void AddSpell(Spell spell, bool takeAbilityScoreOfSpellbook = true)
        {
            spells.Add(spell);
            spell.caster = owner;
            if (takeAbilityScoreOfSpellbook)
            {
                spell.abilityModifier = abilityScore;
            }
            SortSpellsByLevel();
        }

        public void AddSpells(List<Spell> newSpells, bool takeAbilityScoreOfSpellbook = true)
        {
            foreach (Spell spell in newSpells)
            {
                if (!spells.Contains(spell))
                {
                    spells.Add(spell);
                    spell.caster = owner;
                    if (takeAbilityScoreOfSpellbook)
                    {
                        spell.abilityModifier = abilityScore;
                    }

                }
            }
            SortSpellsByLevel();
        }

        public bool HasSpellOfName(string name)
        {
            foreach (Spell spell in spells)
            {
                if (spell.name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}