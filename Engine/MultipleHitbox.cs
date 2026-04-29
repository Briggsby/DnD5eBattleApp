using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BugsbyEngine
{
    public class MultipleHitbox : Hitbox
    {
        List<Hitbox> hitboxes;

        public MultipleHitbox(List<Hitbox> hitboxes, GameObject gameObject) : base(gameObject)
        {
            this.hitboxes = hitboxes;
        }

        public override bool Intersects(Hitbox hitbox)
        {
            foreach (Hitbox hb in hitboxes)
            {
                if (hb.Intersects(hitbox))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CheckClicked()
        {
            foreach (Hitbox hb in hitboxes)
            {
                if (hb.CheckClicked())
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CheckRightClicked()
        {
            foreach (Hitbox hb in hitboxes)
            {
                if (hb.CheckRightClicked())
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CheckMouseOver()
        {
            foreach (Hitbox hb in hitboxes)
            {
                if (hb.CheckMouseOver())
                {
                    return true;
                }
            }
            return false;
        }

        public override Rectangle RectangleHB()
        {
            throw new NotImplementedException();
        }
    }
}