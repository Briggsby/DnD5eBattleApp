namespace DnD5eBattleApp
{
    public class AbilityCheck : Roll
    {
        public Stats stat;
        public string proficiency;

        public bool proficiencyUsed = false;

        public AbilityCheck(Creature creature, Stats stat, string proficiency) : base(creature.encounter)
        {
            this.stat = stat;
            this.proficiency = proficiency;
        }
    }
}