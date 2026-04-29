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
    public class Button : GameObject
    {
        public TextObject textObj;
        public string tag;

        public Button(Vector2 position, string text, SpriteFont font, List<Texture2D> textures, Vector2 size, GameObject parent = null, Vector2? textPosition = null) : base(position, textures[0], parent )
        {
            if (font == null)
            {
                font = EngManager.defaultFont;
            }
            if (text == null) { text = ""; }
            buttonState = ButtonTexture.BaseTexture;
            inActive = false;
            SetTextures(textures);
            transform.SetSize(size, textures[0]);

            textObj = new TextObject(this, text, font, textPosition ?? new Vector2(0, 0), TextRelativePosition.Center);

            MakeHitbox(new Vector2(0, 0), HitboxShape.Rectangle, new Vector3(size.X, size.Y, 0));

        }

        public Button(Vector2 position, string text, SpriteFont font, ButtonTextures textures, Vector2 size, GameObject parent = null, Vector2? textPosition = null) : base(position, textures.baseTexture, parent)
        {

            if (font == null)
            {
                font = EngManager.defaultFont;
            }
            if (text == null) { text = ""; }
            buttonState = ButtonTexture.BaseTexture;
            inActive = false;
            SetTextures(textures);
            transform.SetSize(size, textures.baseTexture);

            textObj = new TextObject(this, text, font, textPosition??new Vector2(0,0), TextRelativePosition.Center);

            MakeHitbox(new Vector2(0, 0), HitboxShape.Rectangle, new Vector3(size.X, size.Y, 0));

        }

        public void SetTextures(List<Texture2D> textures)
        {
            this.textures = new List<Texture2D>(textures);
        }

        public void SetTextures(ButtonTextures textures)
        {
            this.textures = new List<Texture2D>() { textures.baseTexture, textures.hoveredTexture, textures.pressedTexture, textures.pressedHoveredTexture, textures.inactiveTexture };
        }

        public List<Texture2D> textures;
        public List<string> tags;

        public void SetDeActivated()
        {
            pressed = false;
            RecheckTexture();
        }

        public void RecheckTexture()
        {
            if (inActive)
            {
                transform.SetTexture(textures[(int)ButtonTexture.InactiveTexture]);
            }
            else
            {
                if (hovered && pressed)
                {
                    transform.SetTexture(textures[(int)ButtonTexture.PressedHoveredTexture]);
                }
                else if (hovered && !pressed)
                {
                    transform.SetTexture(textures[(int)ButtonTexture.HoveredTexture]);
                }
                else if (!hovered && pressed)
                {
                    transform.SetTexture(textures[(int)ButtonTexture.PressedTexture]);
                }
                else if (!hovered && !pressed)
                {
                    transform.SetTexture(textures[(int)ButtonTexture.BaseTexture]);
                }
            }
        }

        public ButtonTexture buttonState;
        public bool hovered;
        public bool pressed;
        public bool inActive;

        public virtual void Inactivate()
        {
            inActive = true;
            buttonState = ButtonTexture.InactiveTexture;
            transform.SetTexture(textures[(int)ButtonTexture.InactiveTexture]);
        }

        public virtual void MakeActive()
        {
            inActive = false;
            RecheckTexture();
        }

        public override void OnClick()
        {
            base.OnClick();
            if (hitbox == EngManager.controls.topHitbox)
            {
                OnSelect();
            }
        }

        public override void OnMouseOver()
        {
            OnHover();
        }

        public override void OnMouseOff()
        {
             OnUnhover();
        }

        #region Button use functions 

        public virtual void OnHover()
        {


            hovered = true;

            RecheckTexture();

            OnButtonHoverEvent?.Invoke(this, null);

        }

        public virtual void WhileHover()
        {

            WhileButtonHoverEvent?.Invoke(this, null);
        }

        public virtual void OnUnhover()
        {

            OnButtonUnhoverEvent?.Invoke(this, null);
            hovered = false;
            RecheckTexture();
        }

        public virtual void OnSelect()
        {
            if (inActive)
            {
                return;
            }
            OnButtonSelectEvent?.Invoke(this, null);
            pressed = !pressed;
            if (pressed)
            {
                OnActivation();
            }
            if (!pressed)
            {
                OnDeActivation();
            }

        }

        public virtual void OnActivation()
        {
            OnButtonActivationEvent?.Invoke(this, null);
            RecheckTexture();
        }

        public virtual void OnDeActivation()
        {
            OnButtonDeactivationEvent?.Invoke(this, null);
            RecheckTexture();
        }

        #endregion

        #region Button Use Delegates

        public class ButtonEventArgs : EventArgs
        {

        }

        public delegate void ButtonDelegate(Button button, ButtonEventArgs eventArgs);

        public event ButtonDelegate OnButtonActivationEvent;
        public event ButtonDelegate OnButtonDeactivationEvent;
        public event ButtonDelegate OnButtonSelectEvent;
        public event ButtonDelegate OnButtonHoverEvent;
        public event ButtonDelegate WhileButtonHoverEvent;
        public event ButtonDelegate OnButtonUnhoverEvent;

        #endregion

        public override void Update()
        {
            base.Update();
            if (hovered && !inActive)
            {
                WhileHover();
            }
        }
    }
}