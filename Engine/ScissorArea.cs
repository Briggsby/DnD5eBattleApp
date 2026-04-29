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
    public class ScissorArea : ObjectManager
    {
        public Vector2 size;

        public Rectangle Rectangle
        {
            get { return new Rectangle(transform.Position().ToPoint(), size.ToPoint()); }
        }

        public ScissorArea(Vector2 position, Vector2 size, Texture2D texture = null, GameObject parent = null) : base(position, texture, parent)
        {
            this.size = size;
        }

        public void SetHitboxRectangleContainers()
        {
            foreach (GameObject gO in gameObjects)
            {
                if (gO.hitbox != null)
                {
                    gO.hitbox.scissorRectContainer = Rectangle;
                }
            }
        }

        protected override void UpdateGameObjects()
        {
            SetHitboxRectangleContainers();
            base.UpdateGameObjects();
        }

        //THIS ISN'T GOOD, BECAUSE IT STOPS LAYERDEPTH WORKING TO DISTINGUISH THINGS BEFORE AND AFTER THIS DRAWING FUNCTION
        //EVERYTHING DRAWN AFTER WILL BE DRAWN ON TOP OF ANYTHING DRAWN BEFORE REGARDLESS OF LAYER DEPTH
        protected override void DrawGameObjects()
        {
            SpriteBatch newSpriteBatch = new SpriteBatch(EngManager.graphics.GraphicsDevice);
            //RasterizerState oldRasterizer = EngManager.spriteBatch.GraphicsDevice.RasterizerState;
            //EngManager.spriteBatch.End();
            RasterizerState newRasterizer = new RasterizerState();
            newRasterizer.ScissorTestEnable = true;
            newSpriteBatch.Begin(rasterizerState: newRasterizer, sortMode: SpriteSortMode.BackToFront);
            newSpriteBatch.GraphicsDevice.ScissorRectangle = this.Rectangle;
            base.DrawGameObjects();
            newSpriteBatch.End();
            //EngManager.spriteBatch.Begin(rasterizerState: oldRasterizer, sortMode:SpriteSortMode.BackToFront);
        }
    }
}