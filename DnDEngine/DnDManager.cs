using BugsbyEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace DnD5eBattleApp
{
    public enum DamageTypes { BludgeoningMagical, PiercingMagical, SlashingMagical, Bludgeoning, Piercing, Slashing, Fire, Cold, Lightning, Thunder, Psychic, Force, Poison, Acid, Radiant, Necrotic }
    public enum Size { Small, Medium, Large, Huge, Gargantuan };

    #region Stats and Skills
    public enum Stats { Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma, Choose, None };
    public enum Skills { Choose, Athletics, Acrobatics, SleightOfHand, Stealth, Arcana, History, Investigation, Nature, Religion, AnimalHandling, Insight, Medicine, Perception, Survival, Deception, Intimidation, Performance, Persuasion };
    
    #endregion

    public class DnDManager
    {
        public enum Languages { Common, Elvish, Draconic, Dwarvish, Gnomish, Goblin, Halfling, Infernal, Orc, Choose }
        public enum CreatureType { Humanoid, Undead, Beast, Elemental, Fey, Plant}
        public enum CreatureSubType { Goblinoid, Human }

        public static DnDManager manager;

        public static ContextMenu contextMenu;

        public static List<Roll> ongoingRolls;
        public static List<Encounter> encounters;
        public static List<Control> ongoingControls;

        public ContextMenuTextureSet menuTextureSet;
        public BoardTextureSet boardTextureSet;

        public static Dictionary<string, Texture2D> monsterTextures;

        #region Library stuff

        public static List<Library> libraries;

        public static List<string> languages;
        public static Dictionary<string, ItemCreator> items;
        public static Dictionary<string, WeaponCreator> weapons;
        public static Dictionary<string, ArmorCreator> armors;
        public static Dictionary<string, Species> species;
        public static Dictionary<string, SubSpecies> subSpecies;
        public static Dictionary<string, PlayerClass> classes;
        public static Dictionary<string, SubClass> subClasses;
        public static Dictionary<string, FeatCreator> feats;
        public static Dictionary<string, MonsterSpec> monsters;
        public static Dictionary<string, SpellCreator> spells;
        public static Dictionary<string, Dictionary<int,List<string>>> spellLists;
        public static Dictionary<string, ConditionCreator> conditions;


        #region Weapon Categories

        public static List<string> martialMeleeWeapons;
        public static List<string> simpleMeleeWeapons;
        public static List<string> simpleRangedWeapons;
        public static List<string> martialRangedWeapons;

        #endregion

        #endregion

        public DnDManager(Library library, Dictionary<string, Texture2D> monsterTextures, ContextMenuTextureSet menuTextureSet, BoardTextureSet boardTextureSet)
        {
            manager = this;

            ongoingRolls = new List<Roll>();
            encounters = new List<Encounter>();
            ongoingControls = new List<Control>();

            DnDManager.monsterTextures = new Dictionary<string, Texture2D>(monsterTextures);
            this.menuTextureSet = menuTextureSet;
            this.boardTextureSet = boardTextureSet;
            SpellBook.baseContextMenuTextureSet = menuTextureSet;

            InitializeLibraryLists();

            AddLibrary(library);

            contextMenu = null;

        }

        public DnDManager(List<Library> libraries, Dictionary<string, Texture2D> monsterTextures, ContextMenuTextureSet menuTextureSet, BoardTextureSet boardTextureSet)
        {
            manager = this;

            ongoingRolls = new List<Roll>();
            encounters = new List<Encounter>();
            ongoingControls = new List<Control>();

            DnDManager.monsterTextures = new Dictionary<string, Texture2D>(monsterTextures);
            this.menuTextureSet = menuTextureSet;
            this.boardTextureSet = boardTextureSet;
            SpellBook.baseContextMenuTextureSet = menuTextureSet;

            InitializeLibraryLists();

            foreach (Library lib in libraries)
            {
                AddLibrary(lib);
            }

            contextMenu = null;
        }

        public void InitializeLibraryLists()
        {
            libraries = new List<Library>();
            languages = new List<string>();
            items = new Dictionary<string, ItemCreator>();
            armors = new Dictionary<string, ArmorCreator>();
            species = new Dictionary<string, Species>();
            subSpecies = new Dictionary<string, SubSpecies>();
            classes = new Dictionary<string, PlayerClass>();
            subClasses = new Dictionary<string, SubClass>();
            feats = new Dictionary<string, FeatCreator>();
            monsters = new Dictionary<string, MonsterSpec>();
            spells = new Dictionary<string, SpellCreator>();
            spellLists = new Dictionary<string, Dictionary<int, List<string>>>();
            conditions = new Dictionary<string, ConditionCreator>();

            martialMeleeWeapons = new List<string>();
            simpleMeleeWeapons = new List<string>();
            simpleRangedWeapons = new List<string>();
            martialRangedWeapons = new List<string>();
        }

        public void Update()
        {
            RemoveRolls();
            CheckRolls();
            CheckControls();
            CheckContextMenu();
        }

        public void RemoveRolls()
        {
            List<Roll> oldOngoingRolls = new List<Roll>(ongoingRolls);
            foreach (Roll roll in oldOngoingRolls)
            {
                if (roll.finishedRoll)
                {
                    ongoingRolls.Remove(roll);
                }
            }
        }

        public void CheckRolls()
        {
        }

        public void CheckControls()
        {
            List<Control> controlsThisFrame = new List<Control>(ongoingControls);
            foreach (Control control in controlsThisFrame)
            {
                control.Update();
            }
        }

        public void CheckContextMenu()
        {
            if (EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.RightMouseJustDown) && NoBoardHovered())
            {
                if (contextMenu != null)
                {
                    contextMenu.DestroyAndChildren();
                }
                contextMenu = new EncounterControlsContextMenu(EngManager.controls.MousePosition, GetNonEncounterContextMenuTemplate());
            }
        }

        public bool NoBoardHovered()
        {
            foreach (Encounter enc in encounters)
            {
                if (enc.board.hoveredBoardTile != null)
                {
                    return false;
                }
            }

            return true;
        }

        public static Encounter NewEncounter()
        {
            return new Encounter(manager, new Vector2(20, 20), 30, manager.boardTextureSet);
        }

        public ContextMenuTemplate GetNonEncounterContextMenuTemplate()
        {
            ContextMenuTemplate template = new ContextMenuTemplate();
            template.textures = ButtonTextures.FromList(menuTextureSet.blankTileTextures);
            template.font = menuTextureSet.baseFont;

            template.texts = new List<string>() {
                "New Encounter"
            };
            template.tags = new List<List<string>>() {
                new List<string>() {"NewEncounter" }
            };

            template.childMenus = new List<ContextMenuTemplate>() { };

            return template;
        }

        public void AddLibrary(Library newLibrary)
        {
            foreach (string s in newLibrary.languages)
            {
                languages.Add(s);
            }
            foreach (string s in newLibrary.conditions.Keys)
            {
                conditions.Add(s, newLibrary.conditions[s]);
            }
            foreach (string s in newLibrary.items.Keys)
            {
                items.Add(s, newLibrary.items[s]);
            }
            foreach (string s in newLibrary.armors.Keys)
            {
                armors.Add(s, newLibrary.armors[s]);
            }
            foreach (string s in newLibrary.species.Keys)
            {
                species.Add(s, newLibrary.species[s]);
            }
            foreach (string s in newLibrary.subSpecies.Keys)
            {
                subSpecies.Add(s, newLibrary.subSpecies[s]);
            }
            foreach (string s in newLibrary.classes.Keys)
            {
                classes.Add(s, newLibrary.classes[s]);
            }
            foreach (string s in newLibrary.subClasses.Keys)
            {
                subClasses.Add(s, newLibrary.subClasses[s]);
            }
            foreach (string s in newLibrary.feats.Keys)
            {
                feats.Add(s, newLibrary.feats[s]);
            }
            // TODO: Take argument for path
            ingestMonsters("E:/Programming/DnD5eBattleApp/DnDEngine/Libraries/SRD/Monsters");
            foreach (string s in newLibrary.spells.Keys)
            {
                spells.Add(s, newLibrary.spells[s]);
            }
            foreach (string classString in newLibrary.spellLists.Keys)
            {
                if (spellLists.Keys.Contains(classString))
                {
                    foreach (int level in newLibrary.spellLists[classString].Keys)
                    {
                        if (spellLists[classString].Keys.Contains(level))
                        {
                            foreach (string spellString in newLibrary.spellLists[classString][level])
                            {
                                if (!spellLists[classString][level].Contains(spellString))
                                {
                                    spellLists[classString][level].Add(spellString);
                                }
                            }
                        }
                        else
                        {
                            spellLists[classString].Add(level, newLibrary.spellLists[classString][level]);
                        }
                    }
                }
                else
                {
                    spellLists.Add(classString, new Dictionary<int,List<string>>(newLibrary.spellLists[classString]));
                }
            }

            foreach (string s in newLibrary.martialMeleeWeapons)
            {
                martialMeleeWeapons.Add(s);
            }
            foreach(string s in newLibrary.martialRangedWeapons)
            {
                martialRangedWeapons.Add(s);
            }
            foreach (string s in newLibrary.simpleMeleeWeapons)
            {
                simpleMeleeWeapons.Add(s);
            }
            foreach (string s in newLibrary.simpleRangedWeapons)
            {
                simpleRangedWeapons.Add(s);
            }

            libraries.Add(newLibrary);
        }

        private void ingestMonsters(string path)
        {
            // Iterate through every file in path, parse it as a MonsterSpec, and add it to the monsters dictionary with the monster's name as the key
            string[] monsterFiles = System.IO.Directory.GetFiles(path);
            foreach (string file in monsterFiles)
            {
                string json = System.IO.File.ReadAllText(file);
                MonsterSpec spec = System.Text.Json.JsonSerializer.Deserialize<MonsterSpec>(json, SchemaExporter.Options);
                monsters.Add(spec.Name, spec);
            }
        }
    }
}