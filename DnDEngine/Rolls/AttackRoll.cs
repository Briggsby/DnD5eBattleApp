namespace DnD5eBattleApp
{
    public class AttackRoll : Roll
    {
        public Attack attack;

        public AttackRoll(Attack attack) : base(attack.attacker.encounter)
        {
            this.attack = attack;
            GetSource(attack);

            die = 20;
            DC = attack.defender.AC;
            roller = attack.attacker;
            bonus = attack.GetAttackBonus();
            advantage = attack.GetAdvantage();
            disadvantage = attack.GetDisadvantage();

            finishRoll += new RollDelegate(attack.FinishAttackRoll);
        }
    }
}