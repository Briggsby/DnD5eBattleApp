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
    public enum ScrollAreaTextures { ArrowUp, ArrowDown, Scroller, ScrollerBackground, Background };
    public enum ScrollAreaPositions { Body, ArrowUp, ArrowDown, ScrollBar };
    public enum ScrollAreaButtonTypes { ArrowUp, ArrowDown, ScrollBar };

    public class OldScrollArea : ContextMenu
    {
        public List<ButtonTextures> scrollTextureSets;
        public List<Vector2> scrollPositionList;
        public Vector2? sizeScrollBar;
        public Vector2 arrowSize;
        public Vector2 size;

        public int numberOfButtons;

        public Vector2 scrollPosition;

        public bool mouseWheelActive = true;

        public int scrollSpeed = 1;

        public Button upArrow;
        public Button downArrow;

        public OldScrollArea(ContextMenuTemplate menuTemplate, List<Vector2> positions, List<ButtonTextures> textures, Vector2 arrowSize, Vector2 buttonSize, int numberOfButtons, int columns = 1, Vector2? sizeScrollBar = null, GameObject parent = null) : base(menuTemplate, positions[(int)ScrollAreaPositions.Body], parent)
        {
            scrollTextureSets = textures;
            scrollPositionList = positions;
            scrollPosition = new Vector2(0, 0);
            this.arrowSize = arrowSize;

            size.X = (buttonSize.X);
            size.Y = (numberOfButtons / columns) * buttonSize.Y;
            hitbox.shapeVars = new Vector3(size, 0);
            //MoveTo(hitbox.transform.GlobalPosition);

            Debug.WriteLine(hitbox.RectangleHB());

            MakeArrowButtons();
            MakeScroller();

            foreach (ContextMenuItem menuItem in menuItems)
            {
                menuItem.transform.localPosition += new Vector2(0, (int)(numberOfButtons/2)) * buttonSize;
            }
        }

        /*
        public new void GenerateButtons(Vector2? buttonSizeParameter = null)
        {

        }

        public new void GenerateHitbox()
        {

        }

        */

        public void MakeArrowButtons()
        {
            if (scrollTextureSets != null && scrollTextureSets.Count >= (int)ScrollAreaTextures.ArrowUp + 1 && scrollTextureSets[(int)ScrollAreaTextures.ArrowUp] != null)
            {
                if (scrollPositionList != null && scrollPositionList.Count >= (int)ScrollAreaPositions.ArrowUp && scrollPositionList[(int)ScrollAreaPositions.ArrowUp] != null)
                {
                    upArrow = new Button(scrollPositionList[(int)ScrollAreaPositions.ArrowUp], "", null, scrollTextureSets[(int)ScrollAreaTextures.ArrowUp], arrowSize, this);
                    upArrow.transform.layerDepth -= 0.005f;
                    upArrow.OnButtonActivationEvent += new Button.ButtonDelegate(UpArrowPressed);
                }
            }
            if (scrollTextureSets != null && scrollTextureSets.Count >= (int)ScrollAreaTextures.ArrowDown + 1 && scrollTextureSets[(int)ScrollAreaTextures.ArrowDown] != null)
            {
                if (scrollPositionList != null && scrollPositionList.Count >= (int)ScrollAreaPositions.ArrowDown && scrollPositionList[(int)ScrollAreaPositions.ArrowDown] != null)
                {
                    downArrow = new Button(scrollPositionList[(int)ScrollAreaPositions.ArrowDown], "", null, scrollTextureSets[(int)ScrollAreaTextures.ArrowDown], arrowSize, this);
                    downArrow.transform.layerDepth -= 0.005f;
                    downArrow.OnButtonActivationEvent += new Button.ButtonDelegate(DownArrowPressed);
                }
            }
        }
        public void UpArrowPressed(Button button, Button.ButtonEventArgs e)
        {
            Debug.WriteLine("UpPress");
            scrollPosition.Y--;
            foreach (ContextMenuItem menuItem in menuItems)
            {
                menuItem.transform.localPosition += new Vector2(0, 1) * buttonSize;
            }
            button.SetDeActivated();
            InactivateObjectsOutOfScroll();

        }
        public void DownArrowPressed(Button button, Button.ButtonEventArgs e)
        {
            Debug.WriteLine("DownPress");
            scrollPosition.Y++;
            foreach (ContextMenuItem menuItem in menuItems)
            {
                menuItem.transform.localPosition -= new Vector2(0, 1) * buttonSize;
            }
            button.SetDeActivated();
            InactivateObjectsOutOfScroll();

        }
        public void MakeScroller()
        {

        }

        public override void Update()
        {
            base.Update();
            InactivateObjectsOutOfScroll();

        }

        public override void OnSetDestroy()
        {
            base.OnSetDestroy();
            Debug.WriteLine("Destroy");
        }

        public void InactivateObjectsOutOfScroll()
        {
            foreach (ContextMenuItem menuItem in menuItems)
            {
                if (menuItem.hitbox.Intersects(hitbox))
                {
                    menuItem.active = true;
                }
                else
                {
                    menuItem.active = false;
                }
            }
        }

        public override void WhileMouseOver()
        {
            base.WhileMouseOver();
            if (mouseWheelActive)
            {
                scrollPosition.X += EngManager.controls.currentMouseState.ScrollWheelValue - EngManager.controls.previousMouseState.ScrollWheelValue;
            }
        }

    }
}