using System;
using System.Collections.Generic;


namespace DnD5eBattleApp
{
    public class SavingThrow : Roll
    {
        public Stats stat;

        public SavingThrow(Stats stat, Creature creature, Object source, List<string> tags = null) : base(creature.encounter, tags)
        {
            roller = creature;
            this.source = source;
            this.stat = stat;
            bonus = creature.StatMod(stat);
            if (creature.Proficiencies.Contains(stat.ToString()))
            {
                bonus += creature.proficiencyBonus;
            }

            if (source is Spell)
            {
                finishRoll += new RollDelegate((source as Spell).SavingThrowFinished);
            }
        }
    }
}
