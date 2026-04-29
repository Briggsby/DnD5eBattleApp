using System;
using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public class HealingRoll : Roll
    {
        public int numberOfDice;
        public Creature target;

        public HealingRoll(Creature healer, Creature target, int diceFace, int numberOfDice, int bonus, Object source, List<string> tags = null) : base(healer.encounter, tags)
        {
            this.die = diceFace;
            this.numberOfDice = numberOfDice;
            this.bonus = bonus;
            roller = healer;
            this.target = target;

            DoRoll();
        }

        public override int MakeRoll()
        {
            for (int n = 0; n<numberOfDice; n++)
            {
                score += RollD(die);
            }
            score += bonus;
            return score;
        }

        public void Maximum()
        {
            score = (die * numberOfDice) + bonus;
        }

        public override void FinishRoll()
        {
            target.Heal(score);
            base.FinishRoll();
        }
    }
}