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
            EngManager.StartCoroutine(SetOrderControl(
                board: creature.encounter.board,
                originSquare: creature.boardTile,
                range: spell.maxRange,
                color: Color.MistyRose,
                criteria: new List<TileOrderCriteria>() { TileOrderCriteria.WithCreature }
            ));
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            spell.SingleTargetTargeted(orderControl.selection.creature);
        }
    }
}