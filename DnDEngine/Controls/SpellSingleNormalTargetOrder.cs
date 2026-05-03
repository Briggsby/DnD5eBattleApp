using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class SpellSingleNormalTargetOrder : SingleNormalTargetOrder
    {
        Spell Spell {get => ControlObject as Spell; set => ControlObject = value; }

        public SpellSingleNormalTargetOrder(Creature creature, Spell spell) : base(creature, spell, spell.maxRange)
        {
            
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            Spell.SingleTargetTargeted(orderControl.selection.creature);
        }
    }
}