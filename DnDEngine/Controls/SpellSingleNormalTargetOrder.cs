using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class SpellSingleNormalTargetOrder : Control
    {
        Spell spell;
        Creature creature;

        public SpellSingleNormalTargetOrder(Creature creature, Spell spell)
        {
            this.creature = creature;
            this.spell = spell;
            EngManager.StartCoroutine(SetOrderControl(creature.encounter.board, creature.boardTile, spell.maxRange, Color.MistyRose, new List<TileOrderCriteria>() { TileOrderCriteria.WithCreature }));
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            spell.SingleTargetTargeted(orderControl.selection.creature);
        }
    }
}