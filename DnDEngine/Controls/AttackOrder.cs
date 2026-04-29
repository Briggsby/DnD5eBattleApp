using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class AttackOrder : Control
    {
        Creature creature;
        Weapon weapon;
        bool standardAttackAction;
        bool bonusAction;
        bool offHand;

        public AttackOrder(Creature creature, Weapon weapon, bool standardAttackAction = true, bool bonusAction = false, bool offHand = false)
        {
            this.creature = creature;
            this.weapon = weapon;
            this.standardAttackAction = standardAttackAction;
            this.bonusAction = bonusAction;
            this.offHand = offHand;
            EngManager.StartCoroutine(SetOrderControl(creature.encounter.board, creature.boardTile, weapon.maxRange, Color.MistyRose, new List<TileOrderCriteria>() { TileOrderCriteria.WithCreature }));
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