using System;
using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public class SavingThrow : Roll
    {
        public Stat stat;

        public SavingThrow(Stat stat, Creature creature, Object source, List<string> tags = null) : base(creature.encounter, tags)
        {
            roller = creature;
            this.source = source;
            this.stat = stat;
            bonus = creature.StatMod(stat);
            if (creature.Proficiencies.Contains(stat.ToString()))
            {
                bonus += creature.proficiencyBonus;
            }

            if (source is OldSpell)
            {
                finishRoll += new RollDelegate((source as OldSpell).SavingThrowFinished);
            }
        }
    }
}
