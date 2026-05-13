using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public abstract class Library
    {
        public string name;

        public List<string> languages;
        public Dictionary<string, ItemCreator> items;
        public Dictionary<string, WeaponCreator> weapons;
        public Dictionary<string, ArmorCreator> armors;
        public Dictionary<string, Species> species;
        public Dictionary<string, SubSpecies> subSpecies;
        public Dictionary<string, PlayerClass> classes;
        public Dictionary<string, SubClass> subClasses;
        public Dictionary<string, FeatCreator> feats;
        public Dictionary<string, MonsterCreator> monsters;
        public Dictionary<string, ConditionCreator> conditions;


        public List<string> martialMeleeWeapons;
        public List<string> simpleMeleeWeapons;
        public List<string> martialRangedWeapons;
        public List<string> simpleRangedWeapons;

        public Dictionary<string, Dictionary<int, List<string>>> spellLists;

        public Library()
        {
            InitializeLanguages();
            InitializeConditions();
            InitializeItems();
            InitializeArmors();
            InitializeFeats();
            InitializeSpells();
            InitializeSpecies();
            InitializeClasses();
            InitializeMonsters();
            InitializeWeaponLists();
            
        }

        public abstract void InitializeLanguages();

        public abstract void InitializeItems();

        public abstract void InitializeArmors();

        public abstract void InitializeFeats();

        public abstract void InitializeSpells();

        public abstract void InitializeSpecies();

        public abstract void InitializeClasses();

        public abstract void InitializeMonsters();

        public abstract void InitializeConditions();

        public abstract void InitializeWeaponLists();
    }
    }