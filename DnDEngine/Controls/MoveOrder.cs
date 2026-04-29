using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class MoveOrder : Control
    {
        Creature creature;

        public MoveOrder(Creature creature)
        {
            this.creature = creature;
            EngManager.StartCoroutine(SetOrderControl(creature.encounter.board, creature.boardTile, creature.MoveSpeedLeft, Color.Aqua, new List<TileOrderCriteria>() { TileOrderCriteria.WithoutCreature }));
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            EngManager.StartCoroutine(creature.MoveOrder(orderControl.selection));
        }
    }
}