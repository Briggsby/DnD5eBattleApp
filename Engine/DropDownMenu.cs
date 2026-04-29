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
    public class DropDownMenu : TextField
    {
        public ScrollMenuTemplate scrollMenuTemplate;

        public ScrollMenu scrollMenu;
        public ContextMenuItem selection;

        public DropDownMenu(ScrollMenuTemplate scrollMenuTemplate, Vector2 position, ButtonTextures textures, Vector2 size, string defaultText, SpriteFont font = null, bool stretchMenu = true) : base(position,textures, size, font, defaultText, false, false)
        {
            this.scrollMenuTemplate = scrollMenuTemplate;
            scrollMenuTemplate.normalButtonsTemplate.ButtonSize = new Vector2(transform.Size.X/scrollMenuTemplate.normalButtonsTemplate.numberOfColumns, scrollMenuTemplate.normalButtonsTemplate.ButtonSize.Y);
        }

        public override void OnActivation()
        {
            base.OnActivation();
            scrollMenu = new ScrollMenu(scrollMenuTemplate, new Vector2(-transform.Size.X / 2, transform.Size.Y/2+scrollMenuTemplate.UpArrowSize.Y), this);
            scrollMenu.ContextMenuButtonPressEvent += new ContextMenu.ContextMenuDelegate(SelectionMade);
        }
        public override void OnDeActivation()
        {
            base.OnDeActivation();
            if (scrollMenu != null)
            {
                scrollMenu.DestroyAndChildren();
                scrollMenu = null;
            }
        }

        public void SelectionMade(ContextMenuItem item, ContextMenu.ContextMenuEventArgs e)
        {
            scrollMenu.DestroyAndChildren();
            scrollMenu = null;
            textObj.Text = item.textObj.Text;
            selection = item;
            SetDeActivated();
        }


    }
}