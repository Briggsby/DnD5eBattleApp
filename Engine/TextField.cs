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
                        case Keys.Back:
                            if (characterBefore > 0)
                            {
                                characterBefore--;
                                textObj.text = textObj.text.Remove(characterBefore, 1);
                            }
                            break;
                        case Keys.Delete:
                            if (characterBefore < textObj.text.Length)
                                textObj.text = textObj.text.Remove(characterBefore, 1);
                            break;
                        case Keys.Left:
                            if (characterBefore > 0) characterBefore--;
                            break;
                        case Keys.Right:
                            if (characterBefore < textObj.text.Length) characterBefore++;
                            break;
                        case Keys.Space:
                            textObj.text = textObj.text.Insert(characterBefore, " ");
                            characterBefore++;
                            break;
                        default:
                            if (key >= Keys.A && key <= Keys.Z)
                            {
                                textObj.text = textObj.text.Insert(characterBefore, ((char)key).ToString().ToLower());
                                characterBefore++;
                            } else {
                                Debug.WriteLine("Unhandled key: " + key);
                            }
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