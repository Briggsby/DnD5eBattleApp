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
    public class ScrollMenu : ContextMenu
    {
        public ScrollMenuTemplate template;

        public Button upArrow;
        public Button downArrow;
        public Button scrollBar;
        public GameObject scrollBarBackground;

        int scrollPosition;
        public int ScrollPosition
        {
            get { return scrollPosition; }
            set { if (value >= 0 || value <= menuItems.Count - template.numberOfButtons) { scrollPosition = value; RepositionButtons(); } }
        }

        public List<Vector2> buttonBasePositions;

        public ScrollMenu(ScrollMenuTemplate template, Vector2 position, GameObject parent = null, bool leftSide = false, bool destroyOnOtherClick = false) : base(template.normalButtonsTemplate, position, parent, leftSide, destroyOnOtherClick)
        {
            this.template = template;
            upArrow = GenerateUpArrow();
            downArrow = GenerateDownArrow();

            scrollPosition = 0;
            buttonBasePositions = GetBasePositions();
            RepositionButtons();

            GenerateScrollHitbox();
        }

        public Button GenerateUpArrow()
        {
            Button arrow = new Button(template.UpArrowPosition, "", null, template.upArrow, template.UpArrowSize, this);
            arrow.OnButtonActivationEvent += new Button.ButtonDelegate(Up);
            arrow.transform.layerDepth = -0.005f;
            return arrow;
        }

        public Button GenerateDownArrow()
        {
            Button arrow = new Button(template.DownArrowPosition, "", null, template.downArrow, template.DownArrowSize, this);
            arrow.OnButtonActivationEvent += new Button.ButtonDelegate(Down);
            arrow.transform.layerDepth = -0.005f;
            return arrow;
        }

        public List<Vector2> GetBasePositions()
        {
            List<Vector2> positions = new List<Vector2>();
            foreach (ContextMenuItem menuItem in menuItems)
            {
                positions.Add(new Vector2(menuItem.transform.localPosition.X, menuItem.transform.localPosition.Y));
            }

            return positions;
        }

        public void RepositionButtons()
        {

            for (int i = 0; i < menuItems.Count; i++)
            {
                menuItems[i].transform.localPosition = buttonBasePositions[i] - new Vector2(0, (scrollPosition * template.normalButtonsTemplate.ButtonSize.Y));

                if (i < scrollPosition * template.normalButtonsTemplate.numberOfColumns || i >= scrollPosition * template.normalButtonsTemplate.numberOfColumns + template.numberOfButtons)
                {
                    Debug.WriteLine(menuItems[i].textObj.text);
                    menuItems[i].active = false;
                }
                else
                {
                    menuItems[i].active = true;
                }
            }

        }      

        public void GenerateScrollHitbox()
        {
            if (scrollBar == null)
            {
                hitbox = new MultipleHitbox(new List<Hitbox>() { hitbox, upArrow.hitbox, downArrow.hitbox}, this);
            }
            else
            {
                hitbox = new MultipleHitbox(new List<Hitbox>() { hitbox, upArrow.hitbox, downArrow.hitbox, scrollBarBackground.MakeHitbox(), scrollBar.hitbox }, this);
            }
        }

        public void Up(Button button, Button.ButtonEventArgs e)
        {
            ScrollPosition--;
            upArrow.SetDeActivated();
        }

        public void Down(Button button, Button.ButtonEventArgs e)
        {
            ScrollPosition++;
            downArrow.SetDeActivated();
        }
    }
}