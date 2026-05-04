using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class SpellSingleNormalTargetOrder : SingleNormalTargetOrder
    {
        OldSpell Spell {get => ControlObject as OldSpell; set => ControlObject = value; }

        public SpellSingleNormalTargetOrder(Creature creature, OldSpell spell) : base(creature, spell, spell.maxRange)
        {
            
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            Spell.SingleTargetTargeted(orderControl.selection.creature);
        }
    }
}