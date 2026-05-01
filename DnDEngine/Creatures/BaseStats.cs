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
        public List<Stats> savingThrows = new List<Stats>();
        public Dictionary<Stats, int> stats = new Dictionary<Stats, int>() {
            {Stats.Strength, 10 },
            {Stats.Dexterity, 10 },
            {Stats.Constitution, 10 },
            {Stats.Intelligence, 10 },
            {Stats.Wisdom, 10 },
            {Stats.Charisma, 10 }
        };
        public List<Skills> skillProficiencies = new List<Skills>();
        public List<string> toolProficiencies = new List<string>();
        public List<WeaponTypes> weaponTypeProficiencies = new List<WeaponTypes>();
        public List<WeaponCategories> weaponCategoryProficiencies = new List<WeaponCategories>() { WeaponCategories.NaturalWeapon, WeaponCategories.SimpleWeapon };
        public List<ArmorCategories> armorCategoryProficiencies = new List<ArmorCategories>();

        public List<Skills> expertises = new List<Skills>();

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
                hitPointMax += hitDie + StatMod(Stats.Constitution);
                hitDiceNumber -= 1;
            }
            for (int i = 0; i < hitDiceNumber; i++)
            {
                hitPointMax += EngManager.random.Next(1, hitDie + 1) + StatMod(Stats.Constitution);
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

        public void ChangeStat(Stats stat, int change)
        {
            stats[stat] += change;
            if (stat == Stats.Constitution)
            {
                hitPointMax += change * level;
                creature.hitPoints += change * level;
            }
        }

        #endregion 

        #region References

        public Dictionary<Skills, Stats> skillStats = new Dictionary<Skills, Stats>()
        {
            {Skills.Athletics,      Stats.Strength },
            {Skills.Acrobatics,     Stats.Dexterity },
            {Skills.SleightOfHand,  Stats.Dexterity },
            {Skills.Stealth,        Stats.Dexterity },
            {Skills.Arcana,         Stats.Intelligence },
            {Skills.History,        Stats.Intelligence },
            {Skills.Investigation,  Stats.Intelligence },
            {Skills.Nature,         Stats.Intelligence },
            {Skills.Religion,       Stats.Intelligence },
            {Skills.AnimalHandling, Stats.Wisdom },
            {Skills.Insight,        Stats.Wisdom },
            {Skills.Medicine,       Stats.Wisdom },
            {Skills.Perception,     Stats.Wisdom },
            {Skills.Survival,       Stats.Wisdom },
            {Skills.Deception,      Stats.Charisma },
            {Skills.Intimidation,   Stats.Charisma },
            {Skills.Performance,    Stats.Charisma },
            {Skills.Persuasion,     Stats.Charisma }
        };

        public int StatMod(Stats stat)
        {
            return (int)((stats[stat] / 2) - 5);
        }

        #endregion
    }
}