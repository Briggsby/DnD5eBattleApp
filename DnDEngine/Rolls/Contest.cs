namespace DnD5eBattleApp
{
    public class Contest : Roll
    {
        public Contest(Skills skill, Creature initiator, Creature defender) : base(initiator.encounter)
        {

        }
    }
}