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
    public class RectangleHitbox : Hitbox
    {
        public RectangleHitbox(Transform transform, Vector3 shapeVars, GameObject gameObject) : base(gameObject)
        {
            shape = HitboxShape.Rectangle;
            this.transform = transform;
            this.shapeVars = shapeVars;
        }

        public override Rectangle RectangleHB()
        {
            return new Rectangle((int)(transform.Position().X - (shapeVars.X * transform.scale.X / 2)), 
                                 (int)(transform.Position().Y - (shapeVars.Y * transform.scale.Y / 2)), 
                                 (int)(shapeVars.X * transform.scale.X), 
                                 (int)(shapeVars.Y * transform.scale.Y));
        }

        public override bool Intersects(Hitbox hitbox)
        {
            if (hitbox.shape == HitboxShape.Rectangle)
            {
                Rectangle intersect = Rectangle.Intersect(RectangleHB(), hitbox.RectangleHB());
                if (intersect.Width != 0 || intersect.Height != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override bool CheckClicked()
        {
            return EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.LeftMouseJustDown);
        }

        public override bool CheckRightClicked()
        {
            return EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.RightMouseJustDown);
        }

        public override bool CheckMouseOver()
        {
            return RectangleHB().Contains(EngManager.controls.currentMouseState.Position);
        }

    }
}