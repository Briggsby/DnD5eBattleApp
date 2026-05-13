namespace DnD5eBattleApp
{
    public class AttackRoll : Roll
    {
        public Attack attack;

        public AttackRoll(Attack attack) : base(attack.Attacker.Encounter)
        {
            this.attack = attack;
            source = attack.Ability;

            die = 20;
            DC = attack.Defender.AC;
            roller = attack.Attacker;
            bonus = attack.Ability.GetAttackBonus(attack.Attacker);

            // TODO: Implement advantage and disavantage
            // advantage = attack.GetAdvantage();
            // disadvantage = attack.GetDisadvantage();

            finishRoll += new RollDelegate(attack.FinishAttackRoll);
        }
    }
}