namespace DnD5eBattleApp
{
    public class AbilityCheck : Roll
    {
        public Stat stat;
        public string proficiency;

        public bool proficiencyUsed = false;

        public AbilityCheck(Creature creature, Stat stat, string proficiency) : base(creature.Encounter)
        {
            this.stat = stat;
            this.proficiency = proficiency;
        }
    }
}