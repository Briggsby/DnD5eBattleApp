using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;
using System.Collections;


namespace DnD5eBattleApp
{
    public class MoveOrder : Control
    {
        public MoveOrder(Creature creature) : base(creature)
        {
            EngManager.StartCoroutine(SetOrderControl(creature.encounter.board, creature.boardTile, creature.MoveSpeedLeft, Color.Aqua, new List<TileOrderCriteria>() { TileOrderCriteria.WithoutCreature }));
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            EngManager.StartCoroutine(MoveOrderCoroutine(orderControl.selection));
        }

        public IEnumerator MoveOrderCoroutine(BoardTile boardTile)
        {
            List<Creature> creaturesNearNewLocation = new List<Creature>();
            foreach (BoardTile tile in creature.encounter.board.GetTilesInRange(boardTile, 5))
            {
                if (tile.creature != null)
                {
                    creaturesNearNewLocation.Add(tile.creature);
                }
            }

            foreach (BoardTile tile in creature.encounter.board.GetTilesInRange(boardTile, 5))
            {
                if (tile.creature != null && !creaturesNearNewLocation.Contains(tile.creature))
                {
                    if (!tile.creature.reactionTaken)
                    {
                        OpportunityAttackOption optionDisplay = new OpportunityAttackOption(creature, tile.creature);

                        while (!optionDisplay.finished)
                        {
                            yield return null;
                        }
                        if (optionDisplay.cancelled)
                        {
                            yield break;
                        }
                    }
                }
            }
            creature.MoveTo(boardTile);
        }
    }
}