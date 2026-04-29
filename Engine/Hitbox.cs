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
    public enum HitboxShape { Rectangle, Collection }

    public abstract class Hitbox
    {
        public bool active;
        public bool destroy;

        public GameObject gameObject;

        public HitboxShape shape;

        public Transform transform;
        public Vector3 shapeVars;

        public Vector2 Centre
        {
            get { return transform.GlobalPosition; }
        }

        public bool mouseOver;

        public Rectangle scissorRectContainer;

        private bool frameAfterInit;

        public Hitbox(GameObject gameObject)
        {
            this.gameObject = gameObject;
            destroy = false;
            active = true;
            frameAfterInit = false;
        }

        public virtual void FrameAfterInit()
        {
            if (CheckMouseOver())
            {
                OnMouseOver();
            }
        }

        public abstract bool Intersects(Hitbox hitbox);

        public abstract bool CheckMouseOver();

        public abstract bool CheckClicked();

        public abstract bool CheckRightClicked();

        #region Essential, copy and paste to all Hitbox Subclasses
        public virtual void OnClick()
        {
            gameObject.OnClick();
            OnClickEvent?.Invoke(this, null);
        }

        public virtual void WhileClicked()
        {
            gameObject.WhileClicked();
            WhileClickedEvent?.Invoke(this, null);

        }

        public virtual void OnRightClick()
        {
            gameObject.OnRightClick();
            OnRightClickEvent?.Invoke(this, null);

        }

        public virtual void OnMouseOver()
        {
            EngManager.controls.hitboxesMouseOver.Add(this);
            EngManager.controls.FindTopHitbox();
            gameObject.OnMouseOver();
            OnMouseOverEvent?.Invoke(this, null);

        }

        public virtual void OnMouseOff()
        {
            EngManager.controls.hitboxesMouseOver.Remove(this);
            EngManager.controls.FindTopHitbox();
            gameObject.OnMouseOff();
            OnMouseOffEvent?.Invoke(this, null);

        }

        public virtual void WhileMouseOver()
        {
            gameObject.WhileMouseOver();
            WhileMouseOverEvent?.Invoke(this, null);

        }

        public virtual void WhileMouseNotOver()
        {
            gameObject.WhileMouseNotOver();
            WhileMouseNotOverEvent?.Invoke(this, null);

        }

        #endregion

        public virtual void OnSetDestroy()
        {
            EngManager.controls.hitboxesMouseOver.Remove(this);
            EngManager.controls.FindTopHitbox();
            OnDestroyEvent?.Invoke(this, null);

        }
        public virtual void Update()
        {
            if (frameAfterInit)
            {
                frameAfterInit = false;
                FrameAfterInit();
            }

            if (CheckMouseOver())
            {
                if (!mouseOver)
                {
                    OnMouseOver();
                }
                WhileMouseOver();
                mouseOver = true;

                if (CheckClicked())
                {
                    WhileClicked();
                    if (EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.LeftMouseJustDown))
                    {
                        OnClick();
                    }
                }
                if (CheckRightClicked())
                {
                    OnRightClick();
                }
            }
            else
            {
                if (mouseOver)
                {
                    OnMouseOff();
                }
                WhileMouseNotOver();
                mouseOver = false;
            }

        }

        #region Shapes

        public abstract Rectangle RectangleHB();


        #endregion

        #region Events

        public class HitboxEventArgs : EventArgs
        {

        }

        public delegate void HitboxDelegate(Hitbox hitbox, HitboxEventArgs e);
        public delegate void HitboxInteractionDelegate(Hitbox hitbox1, Hitbox hitbox2);

        public HitboxDelegate OnClickEvent;
        public HitboxDelegate WhileClickedEvent;
        public HitboxDelegate OnRightClickEvent;
        public HitboxDelegate OnMouseOverEvent;
        public HitboxDelegate OnMouseOffEvent;
        public HitboxDelegate WhileMouseOverEvent;
        public HitboxDelegate WhileMouseNotOverEvent;
        public HitboxDelegate OnDestroyEvent;

        #endregion
    }
}