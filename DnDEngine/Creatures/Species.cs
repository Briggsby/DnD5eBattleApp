using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public abstract class SpeciesCreator
    {
        public abstract Species CreateSpecies();
    }

    public class Species
    {
        public string name;
        public Dictionary<Stats, int> statIncreases;
        public Size size;
        public int speed;
        public int darkvision;
        public List<string> feats;
        public List<string> languages;
        public List<string> subSpecies;

        public Species()
        {
            name = "NoSpecies";
            statIncreases = new Dictionary<Stats, int>();
            size = Size.Medium;
            speed = 30;
            darkvision = 0;
            feats = new List<string>();
            languages = new List<string>();
            subSpecies = new List<string>();
        }

        public List<Skills> proficiencies = new List<Skills>();

        public virtual void AssignToCharacter(Creature creature)
        {
            //Debug.WriteLine(string.Format("Assigning {0} species", name));
            foreach(Stats stat in statIncreases.Keys)
            {
                creature.baseStats.stats[stat] += statIncreases[stat];
            }
            creature.baseStats.size = size;
            creature.baseStats.speed = speed;
            creature.baseStats.darkvision = darkvision;
            foreach(Skills skill in proficiencies)
            {
                if (!creature.baseStats.skillProficiencies.Contains(skill))
                {
                    creature.baseStats.skillProficiencies.Add(skill);
                }
            }
            foreach (string feat in feats)
            {
                creature.AddFeat(DnDManager.feats[feat].CreateFeat());
            }
            creature.baseStats.languages.AddRange(languages);
        }
    }
}