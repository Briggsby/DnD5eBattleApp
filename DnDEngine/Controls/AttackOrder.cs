using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class AttackOrder : Control
    {
        Weapon weapon;
        bool standardAttackAction;
        bool bonusAction;
        bool offHand;

        public AttackOrder(Creature creature, Weapon weapon, bool standardAttackAction = true, bool bonusAction = false, bool offHand = false) : base(creature)
        {
            this.weapon = weapon;
            this.standardAttackAction = standardAttackAction;
            this.bonusAction = bonusAction;
            this.offHand = offHand;
            EngManager.StartCoroutine(SetOrderControl(
                board: creature.encounter.board, 
                originSquare: creature.boardTile, 
                range: weapon.maxRange,
                color: Color.MistyRose,
                criteria: new List<TileOrderCriteria>() { TileOrderCriteria.WithCreature }
            ));
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            if (standardAttackAction)
            {
                creature.Attack(orderControl.selection.creature, weapon);
            }
            else
            {
                creature.Attack(orderControl.selection.creature, weapon, 1, false, bonusAction, offHand);
            }
        }
    }
}