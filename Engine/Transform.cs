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
    public class Transform
    {
        Transform parent;
        public Transform Parent
        {
            get { return parent; }
        }
        public Vector2 localPosition;
        public Vector2 GlobalPosition
        {
            get { if (parent != null) { return parent.GlobalPosition + localPosition; } else { return localPosition; } }
            set { if (parent != null) { localPosition = value - parent.GlobalPosition; } else { localPosition = value; } }
        }
        public Vector2 velocity;

        public float rotation;
        public float moment;

        Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set { SetTexture(value); }
        }
        public Vector2 origin;
        public Vector2 scale;

        public Color color;

        public Vector2 Size
        {
            get { return new Vector2(texture.Width * scale.X, texture.Height * scale.Y); }
            set { scale = new Vector2(value.X / texture.Width, value.Y / texture.Height); }
        }

        public List<Transform> children;

        public float layerDepth;

        public float LayerDepth
        {
            get { if (parent == null) { return layerDepth; } else { return MathHelper.Clamp(layerDepth + parent.LayerDepth, 0f, 1f); } }
            set { if (parent == null) { layerDepth = value; } else { layerDepth = value - parent.LayerDepth; } }
        }

        public EngManager manager;

        public bool destroy;

        public bool noDraw;

        #region Animation
        public bool animation;

        int elapsedTime;
        int frameTime;
        int frameCount;
        int currentFrame;
        int frameWidth;
        int frameHeight;
        bool animationActive;
        bool looping;
        Rectangle animationSourceRect;

        void InitializeAnimation(int frameTime, int frameWidth, int frameHeight, bool looping)
        {
            elapsedTime = 0;
            this.frameTime = frameTime;
            frameCount = 0;
            currentFrame = 0;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            animationActive = true;
            this.looping = looping;
            animationSourceRect = new Rectangle(0, 0, frameWidth, frameHeight);

        }

        void UpdateAnimation()
        {
            if (!animation || !animationActive)
            {
                return;
            }
            elapsedTime += (int)EngManager.gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime>frameTime)
            {
                currentFrame++;
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    if (looping == false)
                    {
                        animationActive = false;
                    }
                }

                elapsedTime = 0;
            }

            animationSourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
        }


        #endregion

        #region Initializing
        public Transform()
        {
            SetDefaults();
        }

        public Transform(Texture2D texture, Vector2 Position)
        {
            SetDefaults();
            localPosition = Position;
            SetTexture(texture);
        }

        public Transform(Transform parent, Texture2D texture, Vector2 position)
        {
            SetDefaults();

            if (parent != null)
            {
                SetParent(parent, true);
            }
            localPosition = position;

            SetTexture(texture);
        }

        public Transform(Transform parent, Texture2D texture, Vector2 origin, Vector2 position)
        {
            SetDefaults();
            SetParent(parent, true);
            localPosition = position;

            SetTexture(texture, origin);
        }

        #endregion

        #region Setting Variables
        public void SetDefaults()
        {
            destroy = false;

            parent = null;
            localPosition = new Vector2(0, 0);
            velocity = new Vector2(0, 0);

            rotation = 0f;
            moment = 0f;
            layerDepth = 0.2f;

            texture = null;
            origin = new Vector2(0, 0);
            scale = new Vector2(1, 1);

            color = Color.White;

            noDraw = false;

            children = new List<Transform>();
        }

        public void SetParent(Transform parent, bool changePosition = false, float layerDepth = -0.001f)
        {
            if (this.parent != null)
            {
                RemoveParent(changePosition);
            }
            this.parent = parent;
            parent.children.Add(this);

            this.layerDepth = layerDepth;

            if (!changePosition)
            {
                localPosition -= parent.Position();
            }
        }

        public void RemoveParent(bool changePosition = false)
        {
            if (!changePosition)
            {
                localPosition = Position();
            }
            parent.children.Remove(this);
            parent = null;
        }

        public void SetTexture(Texture2D texture)
        {
            this.texture = texture;
        }

        public void SetTexture(Texture2D texture, Vector2 origin)
        {
            this.texture = texture;
            this.origin = origin;
        }

        public void SetSize(Vector2 size, Texture2D texture = null)
        {
            if (texture == null)
            {
                texture = this.texture;
            }
            scale = size / (new Vector2(texture.Width, texture.Height));
        }

        #endregion

        public void Update()
        {
            UpdateAnimation();
            UpdateMovement();
        }

        #region Drawing

        public void Draw(SpriteBatch spriteBatch)
        {
            if (noDraw)
            {
                return;
            }

            if (texture != null && !animation)
            {
                spriteBatch.Draw(texture, GetDestRect(), GetSourceRect(), color, rotation,origin, SpriteEffects.None, LayerDepth);
            }
            else if (texture != null && animation)
            {
                spriteBatch.Draw(texture, GetDestRect(), animationSourceRect, color, rotation, new Vector2(frameWidth*currentFrame, 0) + new Vector2(frameWidth / 2, frameHeight / 2) + origin, SpriteEffects.None, LayerDepth);
            }
        }

        public Rectangle GetDestRect()
        {
            Vector2 position = Position();

            
            return new Rectangle((int)(position.X - ((texture.Width / 2) * scale.X)),
                                 (int)(position.Y - ((texture.Height / 2) * scale.Y)),
                                 (int)(texture.Width * scale.X),
                                 (int)(texture.Height * scale.Y));
        }

        public Rectangle GetSourceRect()
        {
            return new Rectangle(0, 0, texture.Width, texture.Height);
        }

        #endregion

        #region Positions

        public Vector2 Position()
        {
            if (parent == null)
            {
                return localPosition;
            }
            else
            {
                return parent.Position() + localPosition;
            }
        }
        public void UpdateMovement()
        {
            localPosition += velocity;
            rotation += moment;
        }

        #endregion
    }
}
