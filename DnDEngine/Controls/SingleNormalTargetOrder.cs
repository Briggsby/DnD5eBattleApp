using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class SingleNormalTargetOrder : Control
    {
        public virtual Object ControlObject {get; set;}
        Creature creature;

        public SingleNormalTargetOrder(Creature creature, Object controlObject, int range, Color? color = null)
        {
            this.creature = creature;
            this.ControlObject = controlObject;
            EngManager.StartCoroutine(SetOrderControl(creature.encounter.board, creature.boardTile, range, color ?? Color.OrangeRed, new List<TileOrderCriteria>() { TileOrderCriteria.WithCreature }));
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            if (ControlObject is Feat)
            {
                (ControlObject as Feat).SelectionMadeOrder(this);
            }
        }

    }
}