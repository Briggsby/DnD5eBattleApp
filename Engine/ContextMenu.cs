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
    public class ContextMenu : GameObject
    {
        public enum DefaultTags { ParentMenu, DontDestroy, NotButton }

        public const int textBufferDefaultX = 20;
        public const int textBufferDefaultY = 5;

        public List<string> texts;
        public List<List<string>> tags;

        public ButtonTextures textures;

        public SpriteFont font;

        public List<ContextMenuItem> menuItems;

        public ContextMenu parentMenu = null;
        public ContextMenu childMenu;
        public ContextMenuItem childMenuOwner;

        public Vector2 buttonSize;
        public int columns;
        public int leftSide;

        public bool destroyOnOtherClick = false;

        public ContextMenu(Vector2 position, List<Texture2D> textures, List<string> texts, SpriteFont font, List<List<string>> tags, GameObject parent = null, bool destroyOnOtherClick = false) : base(position, null, parent)
        {
            this.texts = texts;
            this.font = font;
            this.tags = tags;
            this.textures = ButtonTextures.FromList(textures);
            this.transform.LayerDepth = EngManager.layerDepths[LayerDepths.ContextMenu];
            this.destroyOnOtherClick = destroyOnOtherClick;

            GenerateButtons();
            GenerateHitbox();

        }

        public ContextMenu(ContextMenuTemplate contextMenuTemplate, Vector2 position, GameObject parent = null, bool leftSide = false, bool destroyOnOtherClick = false) : base(position, null, parent)
        {
            this.texts = contextMenuTemplate.texts;
            this.font = contextMenuTemplate.font;
            this.tags = contextMenuTemplate.tags;
            this.textures = contextMenuTemplate.textures;

            this.destroyOnOtherClick = destroyOnOtherClick;

            this.transform.LayerDepth = EngManager.layerDepths[LayerDepths.ContextMenu];

            this.columns = contextMenuTemplate.numberOfColumns;
            if (columns < 1)
            {
                columns = 1;
            }


            if (leftSide)
            {
                this.leftSide = -1;
            }
            else
            {
                this.leftSide = 1;
            }


            GenerateButtons(contextMenuTemplate.ButtonSize);
            SetInactives(contextMenuTemplate);
            GenerateHitbox();
            GenerateTemplateChildMenus(contextMenuTemplate);

        }

        public ContextMenu(ContextMenuTemplate contextMenuTemplate1, ContextMenuTemplate contextMenuTemplate2, Vector2 position, GameObject parent = null, bool leftSide = false, bool destroyOnOtherClick = false) : base(position, null, parent)
        {
            this.texts = new List<string>(contextMenuTemplate1.texts);
            this.tags = new List<List<string>>(contextMenuTemplate1.tags);
            this.texts.AddRange(contextMenuTemplate2.texts);
            this.tags.AddRange(contextMenuTemplate2.tags);
            this.font = contextMenuTemplate1.font;
            this.textures = contextMenuTemplate1.textures;
            this.transform.LayerDepth = EngManager.layerDepths[LayerDepths.ContextMenu];

            this.destroyOnOtherClick = destroyOnOtherClick;

            this.columns = contextMenuTemplate1.numberOfColumns;
            if (columns < 1)
            {
                columns = 1;
            }

            if (leftSide)
            {
                this.leftSide = -1;
            }
            else
            {
                this.leftSide = 1;
            }

            GenerateButtons(contextMenuTemplate1.ButtonSize);
            GenerateHitbox();
            SetInactives(contextMenuTemplate1);
            SetInactives(contextMenuTemplate2, contextMenuTemplate1.texts.Count);
            GenerateTemplateChildMenus(contextMenuTemplate1);
            GenerateTemplateChildMenus(contextMenuTemplate2, contextMenuTemplate1.texts.Count);

        }

        #region Debug
        public void ContextMenuOnCreationDebug()
        {
            Debug.WriteLine(string.Format("Menu LayerDepth: {0}", transform.LayerDepth));
            Debug.WriteLine(string.Format("Menu layerDepth: {0}", transform.layerDepth));
            Debug.WriteLine(string.Format("Item LayerDepth: {0}", menuItems[0].transform.LayerDepth));
            Debug.WriteLine(string.Format("Item layerDepth: {0}", menuItems[0].transform.layerDepth));
            Debug.WriteLine(string.Format("Item Parent: {0}", menuItems[0].transform.Parent.Texture));
            Debug.WriteLine(string.Format("Text LayerDepth: {0}", menuItems[0].textObjects[0].transform.LayerDepth));
            Debug.WriteLine(string.Format("Text layerDepth: {0}", menuItems[0].textObjects[0].transform.layerDepth));
            Debug.WriteLine(string.Format("Text Parent: {0}", menuItems[0].textObjects[0].transform.Parent.Texture));


        }


        #endregion

        public Vector2 GetButtonAutoSize()
        {
            string longestText = "";
            foreach (string text in texts)
            {
                if (text.Length > longestText.Length)
                {
                    longestText = text;
                }
            }
            return font.MeasureString(longestText) + new Vector2(textBufferDefaultX, textBufferDefaultY);
        }

        public void GenerateButtons(Vector2? buttonSizeOverride = null)
        {
            buttonSize = buttonSizeOverride ?? GetButtonAutoSize();
            menuItems = new List<ContextMenuItem>();

            Vector2 position = buttonSize / 2;
            position.X = position.X * leftSide;

            int i = 0;
            for (int x = 0; x < texts.Count; x += columns)
            {
                for (int c = 0; c < columns; c++)
                {
                    menuItems.Add(new ContextMenuItem(this, tags[i], position + new Vector2(c * buttonSize.X, 0), texts[i], font, textures, buttonSize));
                    if (c == 0 && columns > 1)
                    {
                        menuItems[menuItems.Count - 1].leftSide = true;
                    }
                    i++;
                    if (i >= texts.Count)
                    {
                        break;
                    }
                }

                position.Y += buttonSize.Y;

            }

        }

        public void SetInactives(ContextMenuTemplate template, int append = 0)
        {
            if (template.inactives == null || template.inactives.Count < 1)
            {
                return;
            }
            for (int i = append; i < append + template.inactives.Count; i++)
            {
                if (template.inactives[i - append])
                {
                    menuItems[i].Inactivate();
                }
            }
        }

        public void GenerateHitbox()
        {
            if (columns == 1)
            {
                Vector2 centralPoint = new Vector2(buttonSize.X * columns * leftSide / 2, (buttonSize.Y * texts.Count / columns) / 2);
                MakeHitbox(centralPoint, HitboxShape.Rectangle, new Vector3(buttonSize.X * columns, buttonSize.Y * texts.Count / columns, 0));
            }
            else
            {
                List<Hitbox> hitboxList = new List<Hitbox>();
                foreach (ContextMenuItem item in menuItems)
                {
                    hitboxList.Add(item.hitbox);
                }
                hitbox = new MultipleHitbox(hitboxList, this);
            }
        }

        public virtual void ButtonPress(List<string> tags, ContextMenuItem item)
        {
            ContextMenuButtonPressEvent?.Invoke(item, null);
        }

        public virtual void ButtonHover(List<string> tags, ContextMenuItem item)
        {

        }

        public override void WhileMouseOver()
        {
            base.WhileMouseOver();
            EngManager.controls.mouseOverContextMenu = true;
        }

        public override void WhileMouseNotOver()
        {
            base.WhileMouseNotOver();
        }

        public override void OnMouseOff()
        {
            base.OnMouseOff();
            bool mouseStillOvercontextMenu = false;
            foreach (Hitbox hb in EngManager.controls.hitboxesMouseOver)
            {
                if (hb != this.hitbox && (hb.gameObject.GetType() == typeof(ContextMenu) || hb.gameObject.GetType().IsSubclassOf(typeof(ContextMenu))))
                {
                    mouseStillOvercontextMenu = true;
                }
            }

            if (!mouseStillOvercontextMenu)
            {
                EngManager.controls.mouseOverContextMenu = false;
            }
        }

        public override void OnSetDestroy()
        {
            base.OnSetDestroy();

            bool mouseStillOvercontextMenu = false;
            foreach(Hitbox hb in EngManager.controls.hitboxesMouseOver)
            {
                if (hb != this.hitbox && (hb.gameObject.GetType() == typeof(ContextMenu) || hb.gameObject.GetType().IsSubclassOf(typeof(ContextMenu))))
                {
                    mouseStillOvercontextMenu = true;
                }

            }

            if (!mouseStillOvercontextMenu)
            {
                EngManager.controls.mouseOverContextMenu = false;
            }
        }

        public void GiveChildMenus(string tag, ContextMenuTemplate contextMenuTemplate)
        {
            foreach (ContextMenuItem cmi in menuItems)
            {
                if (cmi.tags.Contains(tag))
                {
                    cmi.GiveChildMenu(contextMenuTemplate);
                }
            }
        }

        public void GenerateTemplateChildMenus(ContextMenuTemplate template, int append = 0)
        {
            if (template.childMenus == null || template.childMenus.Count < 1)
            {
                return;
            }
            for (int i = append; i<append+template.childMenus.Count; i++)
            {
                if (template.childMenus[i-append] != null)
                {
                    ContextMenuTemplate nonnull = template.childMenus[i-append] ?? default(ContextMenuTemplate);
                    menuItems[i].GiveChildMenu(nonnull);
                }
            }
        }

        public void CheckChildMenu()
        {
            if (childMenu == null)
            {
                return;
            }
            else if (!CheckChildMenuHovering())
            {
                DestroyChildMenu(childMenu);
            }
        }

        public void DestroyChildMenu(ContextMenu childMenu)
        {
            childMenu.DestroyAndChildren();
            childMenu.parentMenu.childMenu = null;
            childMenu.parentMenu.childMenuOwner.hoverMenu = null;
        }

        public bool CheckThisOrAnyChildMenuHovered()
        {
            if (childMenu == null)
            {
                return CheckAnyItemHovered();
            }
            else
            {
                return (CheckAnyItemHovered() || childMenu.CheckThisOrAnyChildMenuHovered());
            }
        }

        public bool CheckChildMenuHovering()
        {
            //NonMouse
            if (childMenu == null)
            {
                return true;
            }
            if ((childMenu.parentMenu.childMenuOwner.hovered || childMenu.CheckThisOrAnyChildMenuHovered()))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool CheckAnyItemHovered()
        {
            foreach (ContextMenuItem item in menuItems)
            {
                if (item.hovered)
                {
                    return true;
                }

            }
            return false;
        }

        public ContextMenu FindRootMenu()
        {
            if (parentMenu == null)
            {
                return this;
            }
            else
            {
                return parentMenu.FindRootMenu();
            }
        }

        public override void Update()
        {
            base.Update();
            if (destroyOnOtherClick) { DestroyOnOtherClickCheck(); }
            CheckChildMenu();
        }

        public void DestroyOnOtherClickCheck()
        {
            if (EngManager.controls.assortedInputs.Contains(Controls.AssortedInputs.LeftMouseJustDown) && (!CheckThisOrAnyChildMenuHovered()))
            {
                DestroyAndChildren();
            }
        }


        #region Delegates
        public class ContextMenuEventArgs : EventArgs
        {
        }

        public delegate void ContextMenuDelegate(ContextMenuItem menuItem, ContextMenuEventArgs e);

        public event ContextMenuDelegate ContextMenuButtonPressEvent;

        #endregion
    }
}