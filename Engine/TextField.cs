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
    public class TextField : Button
    {
        public bool editable;
        public bool editOnHover;
        public bool editOnSelect;

        public int characterBefore;

        public float typeDelay = 200;
        public Dictionary<Keys, float> keyDelayValues;

        public TextField(Vector2 position, ButtonTextures textures, Vector2 size, SpriteFont font = null, string defaultText = "Enter Here", bool editOnHover = false, bool editOnSelect = true) : base(position, defaultText, font, textures, size)
        {
            keyDelayValues = new Dictionary<Keys, float>();
            this.editOnHover = editOnHover;
            this.editOnSelect = editOnSelect;
        }

        public override void Update()
        {
            base.Update();
            if (editable)
            {
                KeyboardEdit();
            }
            List<Keys> keyRemoveList = new List<Keys>();
            List<Keys> keyDelayValueKeys = new List<Keys>(keyDelayValues.Keys);
            foreach (Keys key in keyDelayValueKeys)
            {
                keyDelayValues[key] -= EngManager.gameTime.ElapsedGameTime.Milliseconds;
                if (keyDelayValues[key] < 0)
                {
                    keyRemoveList.Add(key);
                }
                else if (!EngManager.controls.currentKeyboardState.GetPressedKeys().Contains(key))
                {
                    keyRemoveList.Add(key);
                }
            }

            foreach (Keys key in keyRemoveList)
            {
                keyDelayValues.Remove(key);
            }
        }
        public override void OnHover()
        {
            base.OnHover();
            if (editOnHover)
            {
                editable = true;
            }
        }
        public override void OnUnhover()
        {
            base.OnUnhover();
            if (editOnHover)
            {
                editable = false;
            }
        }
        public override void OnActivation()
        {
            base.OnActivation();
            if (editOnSelect) { editable = true; characterBefore = textObj.text.Length; }
        }
        public override void OnDeActivation()
        {
            base.OnDeActivation();
            if (editOnSelect) { editable = false; }
        }


        public void KeyboardEdit()
        {
            foreach (Keys key in EngManager.controls.currentKeyboardState.GetPressedKeys())
            {
                #region Regular Keys
                if (!keyDelayValues.Keys.Contains(key))
                {
                    Debug.WriteLine(key);
                    switch (key)
                    {
                        // TODO: Convert this without a massive switch
                        case (Keys.Back):
                            if (characterBefore == 0)
                            {
                                break;
                            }
                            characterBefore--;
                            textObj.text = textObj.text.Remove(characterBefore, 1);
                            break;
                        case (Keys.Delete):
                            if (characterBefore == 0)
                            {
                                break;
                            }
                            textObj.text = textObj.text.Remove(characterBefore, 1);
                            characterBefore--;
                            break;
                        case (Keys.Left):
                            if (characterBefore == 0)
                            {
                                break;
                            }
                            characterBefore--;
                            break;
                        case (Keys.Right):
                            if (characterBefore == textObj.text.Length)
                            {
                                break;
                            }
                            characterBefore++;
                            break;
                        case (Keys.Space):
                            textObj.text = textObj.text.Insert(characterBefore, " ");
                            characterBefore++;
                            break;
                        case (Keys.A):
                            textObj.text = textObj.text.Insert(characterBefore, "a");
                            characterBefore++;
                            break;
                        case (Keys.B):
                            textObj.text = textObj.text.Insert(characterBefore, "b");
                            characterBefore++;
                            break;
                        case (Keys.C):
                            textObj.text = textObj.text.Insert(characterBefore, "c");
                            characterBefore++;
                            break;
                        case (Keys.D):
                            textObj.text = textObj.text.Insert(characterBefore, "d");
                            characterBefore++;
                            break;
                        case (Keys.E):
                            textObj.text = textObj.text.Insert(characterBefore, "e");
                            characterBefore++;
                            break;
                        case (Keys.F):
                            textObj.text = textObj.text.Insert(characterBefore, "f");
                            characterBefore++;
                            break;
                        case (Keys.G):
                            textObj.text = textObj.text.Insert(characterBefore, "g");
                            characterBefore++;
                            break;
                        case (Keys.H):
                            textObj.text = textObj.text.Insert(characterBefore, "h");
                            characterBefore++;
                            break;
                        case (Keys.I):
                            textObj.text = textObj.text.Insert(characterBefore, "i");
                            characterBefore++;
                            break;
                        case (Keys.J):
                            textObj.text = textObj.text.Insert(characterBefore, "j");
                            characterBefore++;
                            break;
                        case (Keys.K):
                            textObj.text = textObj.text.Insert(characterBefore, "k");
                            characterBefore++;
                            break;
                        case (Keys.L):
                            textObj.text = textObj.text.Insert(characterBefore, "l");
                            characterBefore++;
                            break;
                        case (Keys.M):
                            textObj.text = textObj.text.Insert(characterBefore, "m");
                            characterBefore++;
                            break;
                        case (Keys.N):
                            textObj.text = textObj.text.Insert(characterBefore, "n");
                            characterBefore++;
                            break;
                        case (Keys.O):
                            textObj.text = textObj.text.Insert(characterBefore, "o");
                            characterBefore++;
                            break;
                        case (Keys.P):
                            textObj.text = textObj.text.Insert(characterBefore, "p");
                            characterBefore++;
                            break;
                        case (Keys.Q):
                            textObj.text = textObj.text.Insert(characterBefore, "q");
                            characterBefore++;
                            break;
                        case (Keys.R):
                            textObj.text = textObj.text.Insert(characterBefore, "r");
                            characterBefore++;
                            break;
                        case (Keys.S):
                            textObj.text = textObj.text.Insert(characterBefore, "s");
                            characterBefore++;
                            break;
                        case (Keys.T):
                            textObj.text = textObj.text.Insert(characterBefore, "t");
                            characterBefore++;
                            break;
                        case (Keys.U):
                            textObj.text = textObj.text.Insert(characterBefore, "u");
                            characterBefore++;
                            break;
                        case (Keys.V):
                            textObj.text = textObj.text.Insert(characterBefore, "v");
                            characterBefore++;
                            break;
                        case (Keys.W):
                            textObj.text = textObj.text.Insert(characterBefore, "w");
                            characterBefore++;
                            break;
                        case (Keys.X):
                            textObj.text = textObj.text.Insert(characterBefore, "x");
                            characterBefore++;
                            break;
                        case (Keys.Y):
                            textObj.text = textObj.text.Insert(characterBefore, "y");
                            characterBefore++;
                            break;
                        case (Keys.Z):
                            textObj.text = textObj.text.Insert(characterBefore, "z");
                            characterBefore++;
                            break;
                    }
                    AddKeyDelay(key);
                }

                #endregion
            }
        }

        public void AddKeyDelay(Keys key)
        {
            if (keyDelayValues.Keys.Contains(key))
            {
                keyDelayValues[key] = typeDelay;
            }
            else
            {
                keyDelayValues.Add(key, typeDelay);
            }
        }
    }
}