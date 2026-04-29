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

        public List<Skills> proficiencies;
        public List<Stats> stats;

        public Inventory inventory;
    }
}