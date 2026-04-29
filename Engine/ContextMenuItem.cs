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
    public class ContextMenuItem : Button
    {
        public ContextMenu contextMenu;

        public bool childMenu;
        public ContextMenuTemplate hoverMenuTemplate;
        public ContextMenu hoverMenu = null;

        public bool leftSide;

        public ContextMenuItem(ContextMenu contextMenu, List<string> tags, Vector2 position, string text, SpriteFont font, ButtonTextures textures, Vector2 size) : base(position, text, font, textures, size, contextMenu)
        {
            leftSide = false;
            this.contextMenu = contextMenu;
            this.tags = new List<string>(tags);
            this.buttonState = ButtonTexture.BaseTexture;
            childMenu = false;
            transform.layerDepth = -0.001f;

            if (hitbox.CheckMouseOver() && (EngManager.controls.topHitbox == null || (EngManager.controls.topHitbox == hitbox || EngManager.controls.topHitbox.gameObject.transform.LayerDepth > transform.LayerDepth)))
            {
                EngManager.controls.topHitbox = hitbox;
                OnMouseOver();
            }
            else if (hitbox.CheckMouseOver())
            {
                OnMouseOver();
            }
        }

        public ContextMenuItem(ContextMenu contextMenu, List<string> tags, Vector2 position, string text, SpriteFont font, List<Texture2D> textures, Vector2 size) : base(position, text, font, textures, size, contextMenu)
        {
            leftSide = false;
            this.contextMenu = contextMenu;
            this.tags = new List<string>(tags);
            this.buttonState = ButtonTexture.BaseTexture;
            childMenu = false;
            transform.layerDepth = -0.001f;

            if (hitbox.CheckMouseOver() && (EngManager.controls.topHitbox == null || (EngManager.controls.topHitbox == hitbox || EngManager.controls.topHitbox.gameObject.transform.LayerDepth > transform.LayerDepth)))
            {
                EngManager.controls.topHitbox = hitbox;
                OnMouseOver();
            }
            else if (hitbox.CheckMouseOver())
            {
                OnMouseOver();
            }
        }

        public void GiveChildMenu(ContextMenuTemplate hoverMenuTemplate)
        {
            this.hoverMenuTemplate = hoverMenuTemplate;
            childMenu = true;
        }

        public override void OnActivation()
        {
            base.OnActivation();
            contextMenu.FindRootMenu().ButtonPress(tags, this);
        }

        public override void OnHover()
        {
            base.OnHover();

            if (inActive)
            {
                return;
            }

            contextMenu.FindRootMenu().ButtonHover(tags, this);
            if (childMenu && hoverMenu == null)
            {
                MakeChildMenu();
            }
        }

        public void MakeChildMenu()
        {
            if (contextMenu.childMenu != null)
            {
                contextMenu.DestroyChildMenu(contextMenu.childMenu);
            }


            if (!leftSide)
            {
                hoverMenu = new ContextMenu(hoverMenuTemplate, new Vector2(transform.Size.X / 2, -transform.Size.Y / 2), this);
            }
            else
            {
                hoverMenu = new ContextMenu(hoverMenuTemplate, new Vector2((-transform.Size.X / 2), -transform.Size.Y / 2), this, leftSide: true);
            }
            hoverMenu.transform.layerDepth = -0.001f;
            contextMenu.childMenu = hoverMenu;
            contextMenu.childMenuOwner = this;
            contextMenu.childMenu.parentMenu = contextMenu;

        }

        public override void OnUnhover()
        {
            base.OnUnhover();
        }
    }
}