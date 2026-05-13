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

            ApplyMeleeRangeDisadvantage();
            finishRoll += new RollDelegate(attack.FinishAttackRoll);
        }

        public void ApplyMeleeRangeDisadvantage() {
            if (attack.Ability.Targeting.TargetType == TargetType.SingleTargetRanged) {
                int distance = attack.Attacker.Encounter.board.GetDistance(attack.Attacker, attack.Defender);
                if (distance <= 5) {
                    disadvantage = true;
                }
            }
        }
    }
}