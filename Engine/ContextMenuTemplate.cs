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
    public class ContextMenuTemplate
    {
        public ButtonTextures textures;
        public List<string> texts;
        public SpriteFont font;
        public SpriteFont Font
        {
            get { return font ?? EngManager.defaultFont; }
            set { font = value; }
        }
        public List<List<string>> tags;
        public List<ContextMenuTemplate> childMenus;
        public List<bool> inactives;

        public int numberOfColumns = 1;
        public Vector2? _buttonSize;
        public Vector2 ButtonSize
        {
            set { _buttonSize = value; }
            get { return _buttonSize?? GetButtonSize(); }
        }


        public ContextMenuTemplate Copy()
        {
            ContextMenuTemplate copy = new ContextMenuTemplate();
            copy.textures = textures;
            copy.texts = new List<string>(texts);
            copy.font = font;
            copy.tags = new List<List<string>>(tags);
            copy.childMenus = new List<ContextMenuTemplate>(childMenus);
            copy.inactives = new List<bool>(inactives);

            return copy;
        }

        public Vector2 GetButtonSize()
        {
            string longestText = "";
            foreach (string text in texts)
            {
                if (text.Length > longestText.Length)
                {
                    longestText = text;
                }
            }
            return Font.MeasureString(longestText) + new Vector2(ContextMenu.textBufferDefaultX, ContextMenu.textBufferDefaultY);
        }
    }
}