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
    public class Controls
    {
        public KeyboardState previousKeyboardState;
        public KeyboardState currentKeyboardState;

        public GamePadState previousGamePadState;
        public GamePadState currentGamePadState;

        public MouseState previousMouseState;
        public MouseState currentMouseState;

        #region Other States
        public bool mouseOverContextMenu = false;
        #endregion

        #region Hitbox Stuff
        public List<Hitbox> hitboxesMouseOver;
        public Hitbox previousTopHitbox;
        public Hitbox topHitbox;

        public void FindTopHitbox()
        {
            previousTopHitbox = topHitbox;

            if (hitboxesMouseOver.Count > 0)
            {
                topHitbox = hitboxesMouseOver[0];
            }
            foreach (Hitbox hb in hitboxesMouseOver)
            {
                if (hb.gameObject.transform.LayerDepth < topHitbox.gameObject.transform.LayerDepth)
                {
                    topHitbox = hb;
                }
            }

        }

        public List<Hitbox> GetHitboxesByLayerDepth() 
        {
            return hitboxesMouseOver.OrderBy(hb => hb.gameObject.transform.layerDepth).ToList();
        }
        #endregion

        public Vector2 MousePosition
        {
            get { return new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y); }
        }

        public enum AssortedInputs { LeftMouseDown, LeftMouseJustDown, RightMouseJustDown, MouseActive }
        public List<AssortedInputs> assortedInputs;

        public enum Commands { MoveUp, MoveDown, MoveLeft, MoveRight, PrimaryEquipment, Dig }
        public List<Commands> commands;

        public Dictionary<Keys, Commands> keyboardControls;
        public Dictionary<AssortedInputs, Commands> nonKeyboardControls;

        public List<Keys> newKeyPresses;
        public Dictionary<Keys, Commands> newKeyPressControls;

        public Controls(Dictionary<Keys, Commands> keyboardControls = null, Dictionary<AssortedInputs, Commands> nonKeyboardControls = null)
        {
            assortedInputs = new List<AssortedInputs>();
            commands = new List<Commands>();

            hitboxesMouseOver = new List<Hitbox>();

            InitializeControls();
        }

        public void InitializeControls()
        {
            keyboardControls = new Dictionary<Keys, Commands>();
            nonKeyboardControls = new Dictionary<AssortedInputs, Commands>();
            newKeyPressControls = new Dictionary<Keys, Commands>();
        }

        public void InitializeControls(Dictionary<Keys, Commands> keyboardControls = null, Dictionary<AssortedInputs, Commands> nonKeyboardControls = null, Dictionary<Keys,Commands> newKeyPressControls = null)
        {
            if (keyboardControls != null)
            {
                this.keyboardControls = new Dictionary<Keys, Commands>(keyboardControls);
            }
            else
            {
                this.keyboardControls = new Dictionary<Keys, Commands>
                {
                    { Keys.W, Commands.MoveUp },
                    { Keys.A, Commands.MoveLeft },
                    { Keys.S, Commands.MoveDown },
                    { Keys.D, Commands.MoveRight },
                    { Keys.Up, Commands.MoveUp },
                    { Keys.Left, Commands.MoveLeft },
                    { Keys.Down, Commands.MoveDown },
                    { Keys.Right, Commands.MoveRight }
                };
            }

            if (nonKeyboardControls != null)
            {
                this.nonKeyboardControls = new Dictionary<AssortedInputs, Commands>(nonKeyboardControls);
            }
            else
            {
                this.nonKeyboardControls = new Dictionary<AssortedInputs, Commands>
                {
                    { AssortedInputs.LeftMouseDown, Commands.PrimaryEquipment },
                    { AssortedInputs.RightMouseJustDown, Commands.Dig }
                };
            }

            if (newKeyPressControls != null)
            {
                this.newKeyPressControls = new Dictionary<Keys, Commands>(newKeyPressControls);
            }
            else
            {
                this.newKeyPressControls = new Dictionary<Keys, Commands>
                {

                };
            }
        }

        public void Update()
        {
            FindTopHitbox();
            GetInput();
            GetCommands();
        }

        void GetInput()
        {
            previousKeyboardState = currentKeyboardState;
            previousGamePadState = currentGamePadState;
            previousMouseState = currentMouseState;

            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();

            GetAssortedInputs();
            GetNewKeyPresses();
        }

        public void GetAssortedInputs()
        {
            assortedInputs.Clear();

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                assortedInputs.Add(AssortedInputs.LeftMouseDown);
            }
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                assortedInputs.Add(AssortedInputs.LeftMouseJustDown);
            }
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released)
            {
                assortedInputs.Add(AssortedInputs.RightMouseJustDown);
            }
            if (currentMouseState != previousMouseState)
            {
                assortedInputs.Add(AssortedInputs.MouseActive);
            }

        }

        void GetNewKeyPresses()
        {
            newKeyPresses = new List<Keys>();

            Keys[] currentKeyPresses = currentKeyboardState.GetPressedKeys();
            Keys[] previousKeyPresses = previousKeyboardState.GetPressedKeys();

            foreach( Keys k in currentKeyPresses)
            {
                foreach (Keys k2 in previousKeyPresses)
                {
                    if (k == k2)
                    {
                        break;
                    }
                    newKeyPresses.Add(k);
                }
            }
        }

        void GetCommands()
        {
            commands.Clear();

            foreach (Keys key in keyboardControls.Keys)
            {
                if (!commands.Contains(keyboardControls[key]))
                {
                    if (currentKeyboardState.IsKeyDown(key))
                    {
                        commands.Add(keyboardControls[key]);
                    }
                }
            }

            foreach (AssortedInputs assortedInput in nonKeyboardControls.Keys)
            {
                if (!commands.Contains(nonKeyboardControls[assortedInput]))
                {
                    if (assortedInputs.Contains(assortedInput))
                    {
                        commands.Add(nonKeyboardControls[assortedInput]);
                    }
                }
            }

            foreach (Keys key in newKeyPressControls.Keys)
            {
                if (!commands.Contains(newKeyPressControls[key]))
                {
                    if (newKeyPresses.Contains(key))
                    {
                        commands.Add(newKeyPressControls[key]);
                    }
                }
            }
        }

    }
}