using System.Collections.Generic;
using System.Linq;


namespace DnD5eBattleApp
{
    public class DamageRoll : Roll
    {
        public Attack attack;
        public OldSpell spell;
        public Creature Target {get; set;}
        public override int Bonus { get => base.Bonus; set
            {
                bonus += value;
                SortBonus();
            }
        }

        public bool IsAttack { get { return attack != null; } }
        public bool IsSpell { get { return spell != null; } }

        public bool critical;

        public List<int> diceFaces;
        public List<int> numberOfDice;
        public List<string> damageTypes;
        public List<List<int>> diceResults;
        public List<int> scoresPerDie;
        public List<int> naturalScoresPerDie;

        public int TotalScore { get { return scoresPerDie.Sum(); } }

        public DamageRoll(Attack attack) : base(attack.attacker.encounter)
        {
            this.attack = attack;
            roller = attack.attacker;
            source = attack.GetSource();
            attack.GetDamageDice(this);
            bonus = attack.GetDamageBonus();
            critical = attack.attackRoll.Natural == 20;
            finishRoll += new RollDelegate(attack.FinishDamageRoll);
        }

        public DamageRoll(OldSpell spell) : base(spell.caster.encounter)
        {
            this.spell = spell;
            roller = spell.caster;
            source = spell;
            spell.GetDamageDice(this);
            bonus = spell.GetDamageBonus();
            finishRoll += new RollDelegate(spell.FinishDamageRoll);
        }

        public DamageRoll(
            Creature roller, object source, Creature target,
            Damage damage, Encounter encounter,
            RollDelegate finishRoll
        ) : base(encounter)
        {
            this.roller = roller;
            this.source = source;
            Target = target;
            diceFaces = new List<int>() {damage.MaxValueOfDice};
            numberOfDice = new List<int>() {damage.NumberOfDice};
            damageTypes = new List<string>() {damage.DamageType};
            bonus = damage.FlatValue;
            this.finishRoll += finishRoll;
        }

        public override int MakeRoll()
        {
            diceResults = new List<List<int>>();
            scoresPerDie = new List<int>();
            

            for (int d =0; d <diceFaces.Count; d++)
            {
                diceResults.Add(new List<int>());
                scoresPerDie.Add(0);
                for (int n = 0; n<numberOfDice[d]; n++)
                {
                    int roll = RollD(diceFaces[d]);
                    scoresPerDie[d] += roll;
                    diceResults[d].Add(roll);

                }
            }

            naturalScoresPerDie = new List<int>(scoresPerDie);

            SortBonus();
            
            return score = scoresPerDie.Sum();
        }

        public void SortBonus()
        {
            scoresPerDie = new List<int>(naturalScoresPerDie);
            if (critical)
            {
                for (int i = 0; i <scoresPerDie.Count; i++)
                {
                    scoresPerDie[i] *= 2;
                }
            }
            if (bonus < -(scoresPerDie[0]))
            {
                int bonusLeft = bonus;
                while (bonusLeft > 0)
                {
                    if (scoresPerDie.Sum() <= 0)
                    {
                        break;
                    }
                    for (int i = 0; i < scoresPerDie.Count; i++)
                    {
                        if (scoresPerDie[i] != 0)
                        {
                            bonusLeft--;
                            scoresPerDie[i]--;
                            break;
                        }
                    }
                }
            }
            else
            {
                scoresPerDie[0] += bonus;
            }
        }

        public void Reroll(int upTo)
        {
            rerolled = true;
            for (int i = 0; i<diceResults.Count; i++)
            {
                for (int n = 0; n < diceResults[i].Count; n++)
                {
                    if (diceResults[i][n] <= upTo)
                    {
                        naturalScoresPerDie[i] -= diceResults[i][n];
                        diceResults[i][n] = RollD(diceFaces[i]);
                        naturalScoresPerDie[i] += diceResults[i][n];
                    }
                }
            }
            SortBonus();
        }
    }
}