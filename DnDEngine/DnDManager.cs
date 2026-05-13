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
    public enum DamageType { BludgeoningMagical, PiercingMagical, SlashingMagical, Bludgeoning, Piercing, Slashing, Fire, Cold, Lightning, Thunder, Psychic, Force, Poison, Acid, Radiant, Necrotic }
    public enum Size { Small, Medium, Large, Huge, Gargantuan };

    #region Stats and Skills
    public enum Stat { Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma, Choose, None };
    public enum Skill { Choose, Athletics, Acrobatics, SleightOfHand, Stealth, Arcana, History, Investigation, Nature, Religion, AnimalHandling, Insight, Medicine, Perception, Survival, Deception, Intimidation, Performance, Persuasion };
    
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
        public static Order ongoingOrder;

        public ContextMenuTextureSet menuTextureSet;
        public BoardTextureSet boardTextureSet;

        public static Dictionary<string, Texture2D> monsterTextures;

        #region Library stuff

        public static List<string> languages;

        public static Dictionary<string, WeaponSpec> weapons;
        public static Dictionary<string, SpellSpec> spells;
        public static Dictionary<string, MonsterSpec> monsters;
        public static Dictionary<string, ConditionSpec> Conditions {get; set;} = new Dictionary<string, ConditionSpec>();

        public static Dictionary<string, ItemCreator> items;
        public static Dictionary<string, ArmorCreator> armors;
        public static Dictionary<string, Species> species;
        public static Dictionary<string, SubSpecies> subSpecies;
        public static Dictionary<string, PlayerClass> classes;
        public static Dictionary<string, SubClass> subClasses;
        public static Dictionary<string, FeatCreator> oldFeats;
        public static Dictionary<string, Dictionary<int,List<string>>> spellLists;
        public static Dictionary<string, ConditionCreator> oldConditions;


        #region Weapon Categories

        public static List<string> martialMeleeWeapons;
        public static List<string> simpleMeleeWeapons;
        public static List<string> simpleRangedWeapons;
        public static List<string> martialRangedWeapons;

        #endregion

        #endregion

        public DnDManager(Dictionary<string, Texture2D> monsterTextures, ContextMenuTextureSet menuTextureSet, BoardTextureSet boardTextureSet)
        {
            manager = this;

            ongoingRolls = new List<Roll>();
            encounters = new List<Encounter>();
            ongoingOrder = null;

            DnDManager.monsterTextures = new Dictionary<string, Texture2D>(monsterTextures);
            this.menuTextureSet = menuTextureSet;
            this.boardTextureSet = boardTextureSet;

            InitializeLibraryLists();

            AddLibrary();

            contextMenu = null;

        }

        public void InitializeLibraryLists()
        {
            languages = new List<string>();
            items = new Dictionary<string, ItemCreator>();
            armors = new Dictionary<string, ArmorCreator>();
            species = new Dictionary<string, Species>();
            subSpecies = new Dictionary<string, SubSpecies>();
            classes = new Dictionary<string, PlayerClass>();
            subClasses = new Dictionary<string, SubClass>();
            oldFeats = new Dictionary<string, FeatCreator>();
            monsters = new Dictionary<string, MonsterSpec>();
            spells = new Dictionary<string, SpellSpec>();
            spellLists = new Dictionary<string, Dictionary<int, List<string>>>();
            oldConditions = new Dictionary<string, ConditionCreator>();

            martialMeleeWeapons = new List<string>();
            simpleMeleeWeapons = new List<string>();
            simpleRangedWeapons = new List<string>();
            martialRangedWeapons = new List<string>();
        }

        public void Update()
        {
            RemoveRolls();
            CheckRolls();
            CheckOrders();
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

        public void CheckOrders()
        {
            if (ongoingOrder != null)
            {
                ongoingOrder.Update();
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

        public void AddLibrary()
        {
            weapons = new Dictionary<string, WeaponSpec>();
            // TODO: Take argument for path
            var path = "E:/Programming/DnD5eBattleApp/DnDEngine/Libraries/SRD 5.1/";

            SchemaHandler.IngestSpecs(path + "Monsters", monsters);
            SchemaHandler.IngestSpecs(path + "Spells", spells);
            SchemaHandler.IngestSpecs(path + "Conditions", Conditions);
            SchemaHandler.IngestSpecs(path + "Weapons", weapons);
        }

        public static T GetResource<T>(string name)
        {
            TryGetResource<T>(name, out T value);
            return value;
        }

        public static bool TryGetResource<T>(string name, out T value)
        {
            Dictionary<string, T> dict = typeof(T) switch
            {
                Type t when t == typeof(ConditionSpec) => Conditions as Dictionary<string, T>,
                Type t when t == typeof(MonsterSpec) => monsters as Dictionary<string, T>,
                Type t when t == typeof(SpellSpec) => spells as Dictionary<string, T>,
                Type t when t == typeof(WeaponSpec) => weapons as Dictionary<string, T>,
                _ => throw new SystemException($"No matching resource in library of type {typeof(T)}")
            };

            if (!dict.TryGetValue(name, out T result))
            {
                Console.WriteLine($"No resource of {typeof(T)} named {name} found");
                value = result;
                return false;
            }
            value = result;
            return true;
        }
    }
}