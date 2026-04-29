namespace DnD5eBattleApp
{
    public class InitiativeRoll : Roll
    {
        public InitiativeRoll(Creature creature) : base(creature.encounter)
        {

        }
    }
}