using System.Collections.Generic;

using BugsbyEngine;


namespace DnD5eBattleApp
{ 
    public class BaseStats
    {
        public Creature creature;

        public string creatureType = "Humanoid";
        public string creatureSubType = "Human";
        public Alignment alignment = Alignment.Neutral;
        public Size size = Size.Medium;
        public int level = 1;

        public int naturalAC = 10;
        public int acBonus = 0;

        public int hitDie = 8;
        public int hitDiceNumber = 1;
        public int hitPointMax = 8;
        public int darkvision = 0;
        public List<string> languages = new List<string>() { "Common" };

        public int attacks = 1;
        public int attackRange = 5;
        public List<Weapon> naturalWeapons = new List<Weapon>() { new Weapon() };

        public int proficiencyBonus = 2;
        public List<Stat> savingThrows = new List<Stat>();
        public Dictionary<Stat, int> stats = new Dictionary<Stat, int>() {
            {Stat.Strength, 10 },
            {Stat.Dexterity, 10 },
            {Stat.Constitution, 10 },
            {Stat.Intelligence, 10 },
            {Stat.Wisdom, 10 },
            {Stat.Charisma, 10 }
        };
        public List<Skill> skillProficiencies = new List<Skill>();
        public List<string> toolProficiencies = new List<string>();
        public List<WeaponType> weaponTypeProficiencies = new List<WeaponType>();
        public List<WeaponCategory> weaponCategoryProficiencies = new List<WeaponCategory>() { WeaponCategory.NaturalWeapon, WeaponCategory.SimpleWeapon };
        public List<ArmorCategories> armorCategoryProficiencies = new List<ArmorCategories>();

        public List<Skill> expertises = new List<Skill>();

        public List<string> vulnerabilities = new List<string>();
        public List<string> resistances = new List<string>();
        public List<string> immunities = new List<string>();


        public int speed = 30;
        public int flySpeed = 0;
        public int burrowSpeed = 0;
        public int swimSpeed = 0;

        public void RollHP(bool maxFirstDie = false)
        {
            hitPointMax = 0;
            if (maxFirstDie)
            {
                hitPointMax += hitDie + StatMod(Stat.Constitution);
                hitDiceNumber -= 1;
            }
            for (int i = 0; i < hitDiceNumber; i++)
            {
                hitPointMax += EngManager.random.Next(1, hitDie + 1) + StatMod(Stat.Constitution);
            }
        }

        public BaseStats(Creature creature = null, bool rollHP = true)
        {
            this.creature = creature;
            if (rollHP)
            {
                RollHP();
            }
        }

        public BaseStats(MonsterSpec spec, Creature creature, bool rollHP = true)
        {
            this.creature = creature;
            proficiencyBonus = spec.ProficiencyBonus;
            creatureType = spec.Type;
            creatureSubType = spec.SubType;
            alignment = spec.Alignment;
            size = spec.Size;
            naturalAC = spec.ArmorClass;
            hitDie = spec.HitDiceValue;
            hitDiceNumber = spec.HitDiceNumber;
            speed = spec.Speed;
            darkvision = spec.Darkvision;
            languages = new List<string>(spec.Languages);
            stats[Stat.Strength] = spec.Stats.Strength;
            stats[Stat.Dexterity] = spec.Stats.Dexterity;
            stats[Stat.Constitution] = spec.Stats.Constitution;
            stats[Stat.Intelligence] = spec.Stats.Intelligence;
            stats[Stat.Wisdom] = spec.Stats.Wisdom;
            stats[Stat.Charisma] = spec.Stats.Charisma;

         if (rollHP)
            {
                RollHP();
            }
        }

        #region Recalibrating
        public int AssignLevelProficiencyBonus()
        {
            if (creature is PlayerCharacter)
            {
                if (level < 5)
                {
                    return proficiencyBonus = 2;
                }
                else if (level < 9)
                {
                    return proficiencyBonus = 3;
                }
                else if (level < 13)
                {
                    return proficiencyBonus = 4;
                }
                else if (level < 17)
                {
                    return proficiencyBonus = 5;
                }
                else
                {
                    return proficiencyBonus = 6;
                }
            }
            else
            {
                return proficiencyBonus;
            }
        }

        #endregion

        #region Stats

        public void ChangeStat(Stat stat, int change)
        {
            stats[stat] += change;
            if (stat == Stat.Constitution)
            {
                hitPointMax += change * level;
                creature.hitPoints += change * level;
            }
        }

        #endregion 

        #region References

        public Dictionary<Skill, Stat> skillStats = new Dictionary<Skill, Stat>()
        {
            {Skill.Athletics,      Stat.Strength },
            {Skill.Acrobatics,     Stat.Dexterity },
            {Skill.SleightOfHand,  Stat.Dexterity },
            {Skill.Stealth,        Stat.Dexterity },
            {Skill.Arcana,         Stat.Intelligence },
            {Skill.History,        Stat.Intelligence },
            {Skill.Investigation,  Stat.Intelligence },
            {Skill.Nature,         Stat.Intelligence },
            {Skill.Religion,       Stat.Intelligence },
            {Skill.AnimalHandling, Stat.Wisdom },
            {Skill.Insight,        Stat.Wisdom },
            {Skill.Medicine,       Stat.Wisdom },
            {Skill.Perception,     Stat.Wisdom },
            {Skill.Survival,       Stat.Wisdom },
            {Skill.Deception,      Stat.Charisma },
            {Skill.Intimidation,   Stat.Charisma },
            {Skill.Performance,    Stat.Charisma },
            {Skill.Persuasion,     Stat.Charisma }
        };

        public int StatMod(Stat stat)
        {
            return (int)((stats[stat] / 2) - 5);
        }

        #endregion
    }
}