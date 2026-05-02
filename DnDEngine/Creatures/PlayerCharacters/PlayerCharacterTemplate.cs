using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public class PlayerCharacterTemplate
    {
        public Dictionary<PlayerClass, int> classes;
        public List<SubClass> subclasses;
        public Species species;
        public SubSpecies subSpecies;
        public Background background;

        public List<Skill> proficiencies;
        public List<Stat> stats;

        public Inventory inventory;
    }
}