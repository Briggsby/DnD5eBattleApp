namespace DnD5eBattleApp
{
    public class Contest : Roll
    {
        public Contest(Skill skill, Creature initiator, Creature defender) : base(initiator.encounter)
        {

        }
    }
}