using System;
using System.Collections.Generic;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class OldSpellBook
    {
        public Stat abilityScore;
        public static ContextMenuTextureSet baseContextMenuTextureSet;
        public ContextMenuTextureSet contextMenuTextures;
        public List<OldSpell> spells;
        public Dictionary<int, List<OldSpell>> spellsByLevel;
        public Creature owner;

        public OldSpellBook()
        {
            spells = new List<OldSpell>();
            spellsByLevel = BlankSpellsByLevel();
            contextMenuTextures = baseContextMenuTextureSet;
        }

        public OldSpellBook(Creature creature)
        {
            spells = new List<OldSpell>();
            spellsByLevel = BlankSpellsByLevel();
            contextMenuTextures = baseContextMenuTextureSet;
            owner = creature;
        }

        public OldSpellBook(List<OldSpell> spells)
        {
            if (spells == null)
            {
                spells = new List<OldSpell>();
                spellsByLevel = BlankSpellsByLevel();
            }
            this.spells = new List<OldSpell>(spells);
            spellsByLevel = SortSpellsByLevel(spells);
            contextMenuTextures = baseContextMenuTextureSet;
        }

        public OldSpellBook(SpellBookSpec spellBookSpec, Creature owner)
        {
            this.owner = owner;
            spells = new List<OldSpell>();
            foreach (string spellName in spellBookSpec.Spells)
            {
                if (DnDManager.spells.ContainsKey(spellName))
                {
                    spells.Add(new OldSpell(DnDManager.spells[spellName], owner));
                }
                else
                {
                    Console.WriteLine(string.Format("Spell {0} not found in DnDManager.spells", spellName));
                }
            }
            spellsByLevel = SortSpellsByLevel(spells);
            contextMenuTextures = baseContextMenuTextureSet;
        }

        public OldSpellBook(List<OldSpell> spells, Creature creature, Stat spellcastingStat = Stat.None)
        {
            contextMenuTextures = baseContextMenuTextureSet;
            if (spells == null)
            {
                spells = new List<OldSpell>();
            }
            this.spells = new List<OldSpell>(spells);
            spellsByLevel = SortSpellsByLevel();
            AssignOwner(creature);

            if (spellcastingStat != Stat.None)
            {
                foreach( OldSpell spell in spells)
                {
                    spell.abilityModifier = spellcastingStat;
                }
            }
        }

        public OldSpellBook(List<OldSpell> spells, Dictionary<int, List<OldSpell>> spellsByLevel, ContextMenuTextureSet contextMenuTextures)
        {
            this.spells = new List<OldSpell>(spells);
            this.spellsByLevel = new Dictionary<int, List<OldSpell>>(spellsByLevel);
            this.contextMenuTextures = contextMenuTextures;
        }

        public OldSpellBook(OldSpellBook copy)
        {
            spells = new List<OldSpell>(copy.spells);
            spellsByLevel = new Dictionary<int, List<OldSpell>>(copy.spellsByLevel);
            contextMenuTextures = copy.contextMenuTextures;
        }

        public Dictionary<int, List<OldSpell>> BlankSpellsByLevel()
        {
            return new Dictionary<int, List<OldSpell>>()
            {
                {0, new List<OldSpell>() },
                {1, new List<OldSpell>() },
                {2, new List<OldSpell>() },
                {3, new List<OldSpell>() },
                {4, new List<OldSpell>() },
                {5, new List<OldSpell>() },
                {6, new List<OldSpell>() },
                {7, new List<OldSpell>() },
                {8, new List<OldSpell>() },
                {9, new List<OldSpell>() },
            };
        }

        public static Dictionary<int, List<OldSpell>> SortSpellsByLevelStatic(List<OldSpell> spells)
        {
            Dictionary<int, List<OldSpell>> spellsSortedByLevel = new Dictionary<int, List<OldSpell>>
            {
                {0, new List<OldSpell>() },
                {1, new List<OldSpell>() },
                {2, new List<OldSpell>() },
                {3, new List<OldSpell>() },
                {4, new List<OldSpell>() },
                {5, new List<OldSpell>() },
                {6, new List<OldSpell>() },
                {7, new List<OldSpell>() },
                {8, new List<OldSpell>() },
                {9, new List<OldSpell>() }
            };
            foreach (OldSpell spell in spells)
            {
                spellsSortedByLevel[spell.spellLevel].Add(spell);
            }
            return spellsSortedByLevel;
        }

        public Dictionary<int, List<OldSpell>> SortSpellsByLevel(List<OldSpell> spells = null)
        {
            List<OldSpell> spellsToSort;
            if (spells != null)
            {
                spellsToSort = new List<OldSpell>(spells);
            }
            else
            {
                spellsToSort = new List<OldSpell>(this.spells);
            }
            Dictionary<int,List<OldSpell>> spellsSortedByLevel = new Dictionary<int, List<OldSpell>>
            {
                {0, new List<OldSpell>() },
                {1, new List<OldSpell>() },
                {2, new List<OldSpell>() },
                {3, new List<OldSpell>() },
                {4, new List<OldSpell>() },
                {5, new List<OldSpell>() },
                {6, new List<OldSpell>() },
                {7, new List<OldSpell>() },
                {8, new List<OldSpell>() },
                {9, new List<OldSpell>() }
            };
            foreach (OldSpell spell in spellsToSort)
            {
                spellsSortedByLevel[spell.spellLevel].Add(spell);
            }
            if (spells == null)
            {
                spellsByLevel = spellsSortedByLevel;
            }
            return spellsSortedByLevel;
        }

        public OldSpellBook CopyNewSpellBook(bool alsoTextures = false)
        {
            return new OldSpellBook(spells);
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

            foreach (OldSpell spell in spells)
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

        public bool ContainsSpellLevel(int i, List<OldSpell> spellList = null)
        {
            Dictionary<int, List<OldSpell>> spellsSortedByLevel;
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
            foreach(OldSpell spell in spells)
            {
                spell.caster = creature;
                spell.spellbook = this;
            }
        }

        public void AddSpell(OldSpell spell, bool takeAbilityScoreOfSpellbook = true)
        {
            spells.Add(spell);
            spell.caster = owner;
            if (takeAbilityScoreOfSpellbook)
            {
                spell.abilityModifier = abilityScore;
            }
            SortSpellsByLevel();
        }

        public void AddSpells(List<OldSpell> newSpells, bool takeAbilityScoreOfSpellbook = true)
        {
            foreach (OldSpell spell in newSpells)
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
            foreach (OldSpell spell in spells)
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