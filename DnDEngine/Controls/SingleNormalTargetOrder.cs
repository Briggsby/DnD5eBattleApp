using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using BugsbyEngine;


namespace DnD5eBattleApp
{
    public class SingleNormalTargetOrder : Control
    {
        public virtual Object ControlObject {get; set;}

        public SingleNormalTargetOrder(Creature creature, Object controlObject, int range, Color? color = null) : base(creature)
        {
            this.ControlObject = controlObject;
            EngManager.StartCoroutine(SetOrderControl(creature.encounter.board, creature.boardTile, range, color ?? Color.LightPink, new List<TileOrderCriteria>() { TileOrderCriteria.WithCreature }));
        }

        public override void SelectionMade()
        {
            base.SelectionMade();
            if (ControlObject is OldFeat)
            {
                (ControlObject as OldFeat).SelectionMadeOrder(this);
            } else if (ControlObject is Ability)
            {
                (ControlObject as Ability).SelectionMade(this);
            }
        }

    }
}