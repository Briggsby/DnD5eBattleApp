using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public class SubSpecies
    {
        public string name;
        public string parentSpecies;

        public Dictionary<Stat, int> statIncreases;
        public List<string> feats;
        public List<string> languages;

        public SubSpecies()
        {
            statIncreases = new Dictionary<Stat, int>();
            feats = new List<string>();
            languages = new List<string>();
        }

        public virtual void AssignToCharacter(Creature creature)
        {
            //Debug.WriteLine(string.Format("Assigning {0} subspecies", name));
            foreach (Stat stat in statIncreases.Keys)
            {
                creature.baseStats.stats[stat] += statIncreases[stat];
            }
            foreach (string feat in feats)
            {
                creature.AddFeat(DnDManager.feats[feat].CreateFeat());
            }
            creature.baseStats.languages.AddRange(languages);
        }
    }
}