using System.Linq;
using System.Collections.Generic;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public enum QuickCharacterChoices { Species, SubSpecies, Class, SubClass, Level}

    public class PlayerCharacter : Creature
    {
        public PlayerCharacter(List<string> quickCharacterChoices, BoardTile tile) : base(tile)
        {

            classes = new Dictionary<PlayerClass, int>() { { DnDManager.classes[quickCharacterChoices[(int)QuickCharacterChoices.Class]], int.Parse(quickCharacterChoices[(int)QuickCharacterChoices.Level]) } };
            primaryClass = DnDManager.classes[quickCharacterChoices[(int)QuickCharacterChoices.Class]];
            subclasses = new List<SubClass>() { DnDManager.subClasses[quickCharacterChoices[(int)QuickCharacterChoices.SubClass]] };
            species = DnDManager.species[quickCharacterChoices[(int)QuickCharacterChoices.Species]];
            if (quickCharacterChoices[(int)QuickCharacterChoices.SubSpecies] != null)
            {
                subSpecies = DnDManager.subSpecies[quickCharacterChoices[(int)QuickCharacterChoices.SubSpecies]];
            }
            else
            {
                subSpecies = null;
            }
            level = int.Parse(quickCharacterChoices[(int)QuickCharacterChoices.Level]);

            if (subSpecies != null)
            {
                Name = string.Format("Lvl " + quickCharacterChoices[(int)QuickCharacterChoices.Level] + " " + quickCharacterChoices[(int)QuickCharacterChoices.SubSpecies] + " " + quickCharacterChoices[(int)QuickCharacterChoices.SubClass]);
            }
            else
            {
                Name = string.Format("Lvl " + quickCharacterChoices[(int)QuickCharacterChoices.Level] + " " + quickCharacterChoices[(int)QuickCharacterChoices.Species] + " " + quickCharacterChoices[(int)QuickCharacterChoices.SubClass]);
            }

            RollStats(true);
            MakePlayerCharacter();
            baseStats.RollHP(true);
            ResetHP();
            RecalibrateStats();
        }

        public void MakePlayerCharacter()
        {
            species.AssignToCharacter(this);

            if (subSpecies != null)
            {
                subSpecies.AssignToCharacter(this);
            }

            primaryClass.AssignToCharacter(this, classes[primaryClass]);
            foreach(PlayerClass pC in classes.Keys)
            {
                if (pC != primaryClass)
                {
                    pC.AssignMultiClassToCharacter(this, classes[pC]);
                }
            }
            foreach (SubClass sC in subclasses)
            {
                sC.AssignToCharacter(this, classes[DnDManager.classes[sC.parentClass]]);
            }
            
            
        }

        public void RollStats(bool informedByClass = false)
        {
            List<int> rolls = new List<int>();

            for (int i = 0; i<6; i++)
            {
                List<int> d6s = new List<int>();
                for (int d = 0; d<4; d++)
                {
                    d6s.Add(EngManager.random.Next(1, 7));
                }
                d6s.Remove(d6s.Min());

                rolls.Add(d6s.Sum());
            }

            if (informedByClass && classes != null)
            {
                baseStats.stats[primaryClass.statRelevanceInOrder[0]] = rolls.Max();        
                rolls.Remove(rolls.Max());                                                  
                baseStats.stats[primaryClass.statRelevanceInOrder[1]] = rolls.Max();        
                rolls.Remove(rolls.Max());
                baseStats.stats[primaryClass.statRelevanceInOrder[2]] = rolls.Max();
                rolls.Remove(rolls.Max());
                baseStats.stats[primaryClass.statRelevanceInOrder[3]] = rolls.Max();
                rolls.Remove(rolls.Max());
                baseStats.stats[primaryClass.statRelevanceInOrder[4]] = rolls.Max();
                rolls.Remove(rolls.Max());
                baseStats.stats[primaryClass.statRelevanceInOrder[5]] = rolls.Max();
                rolls.Remove(rolls.Max());
            }
            else
            {
                baseStats.stats[Stat.Strength] = rolls[0];
                baseStats.stats[Stat.Dexterity] = rolls[1];
                baseStats.stats[Stat.Constitution] = rolls[2];
                baseStats.stats[Stat.Intelligence] = rolls[3];
                baseStats.stats[Stat.Wisdom] = rolls[4];
                baseStats.stats[Stat.Charisma] = rolls[5];
            }
        }


    }
    }