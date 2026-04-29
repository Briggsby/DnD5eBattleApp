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
    public class ScrollMenuTemplate
    {
        public ContextMenuTemplate normalButtonsTemplate;
        public int numberOfButtons;
        public ButtonTextures upArrow;
        Vector2? upArrowPosition;
        public Vector2 UpArrowPosition
        {
            set { upArrowPosition = value; }
            get { return upArrowPosition ?? new Vector2(UpArrowSize.X / 2, -UpArrowSize.Y / 2); }
        }
        Vector2? upArrowSize;
        public Vector2 UpArrowSize
        {
            set { upArrowSize = value; }
            get { return upArrowSize ?? new Vector2(normalButtonsTemplate.ButtonSize.X*normalButtonsTemplate.numberOfColumns, normalButtonsTemplate.ButtonSize.Y); }
        }
        public ButtonTextures downArrow;
        Vector2? downArrowPosition;
        public Vector2 DownArrowPosition
        {
            set { downArrowPosition = value; }
            get { return downArrowPosition ?? new Vector2(DownArrowSize.X / 2, (normalButtonsTemplate.ButtonSize.Y * (numberOfButtons / normalButtonsTemplate.numberOfColumns)) + DownArrowSize.Y / 2); }
        }
        Vector2? downArrowSize;
        public Vector2 DownArrowSize
        {
            set { downArrowSize = value; }
            get { return downArrowSize ?? new Vector2(normalButtonsTemplate.ButtonSize.X * normalButtonsTemplate.numberOfColumns, normalButtonsTemplate.ButtonSize.Y); }
        }
        public ButtonTextures scroller;
        public Texture2D scrollBarBackground;
        Vector2? scrollBarPosition;
        public Vector2 ScrollBarPosition
        {
            set { scrollBarPosition = value; }
            get
            {
                return scrollBarPosition ?? new Vector2(normalButtonsTemplate.numberOfColumns * normalButtonsTemplate.ButtonSize.X + ScrollBarSize.X / 2,
                                    ((numberOfButtons / normalButtonsTemplate.numberOfColumns) * normalButtonsTemplate.ButtonSize.Y) / 2);
            }
        }
        public Vector2? scrollBarSize;
        public Vector2 ScrollBarSize
        {
            set { scrollBarSize = value; }
            get
            {
                return scrollBarSize ?? new Vector2(scrollBarBackground.Width, scrollBarBackground.Height);

            }
        } 

        public ScrollMenuTemplate(ContextMenuTemplate menuTemplate)
        {
            normalButtonsTemplate = menuTemplate;

            upArrow = null;
            upArrowPosition = null;
            upArrowSize = null;
            downArrow = null;
            downArrowPosition = null;
            downArrowSize = null;
            scroller = null;
            scrollBarBackground = null;
        }
    }
    }