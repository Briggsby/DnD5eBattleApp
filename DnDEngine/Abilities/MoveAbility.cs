using System.Collections;
using System.Collections.Generic;
using BugsbyEngine;

namespace DnD5eBattleApp;

class MoveAbility : Ability {
    public MoveAbility(int speed) : base(
        name: "Move",
        targeting: new Targeting{
            TargetType=TargetType.EmptyTile,
            Range=speed,
            Size=1,
            HasAttackRoll=false
        },
        actionType: ActionType.Movement,
        damage: null,
        appliesCondition: null
    ) {
    }

    public override void SelectionMade(Order order)
    {
        base.SelectionMade(order);
        EngManager.StartCoroutine(AttemptMoveCoroutine(order.Creature, order.Selection));
    }

    public IEnumerator AttemptMoveCoroutine(Creature creature, BoardTile boardTile)
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