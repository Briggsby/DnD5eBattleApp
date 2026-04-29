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
    public class DisplayOptions : GameObject
    {
        public static DisplayOptionsTextureSet baseDisplayOptionsTextureSet;

        public string question;
        public DisplayOptionsTextureSet textureSet;
        public bool yesAndNo;
        public bool haveCancelButton;
        public bool haveScrollOptions;
        public List<string> availableOptions;
        public bool haveAdditionalScrollOptions;
        public List<List<string>> additionalAvailableOptions;

        public TextObject questionTextObj;
        public ScrollMenu availableOptionScroller;
        public Button yesButton;
        public Button noButton;
        public Button cancelButton;

        public List<ScrollMenu> additionalScrollers;

        public bool finished;
        public bool cancelled;


        public DisplayOptions(string question, DisplayOptionsTextureSet textureSet, bool yesAndNo = true, bool haveCancelButton = true, bool haveScrollOptions = false, List<string> availableOptions = null, bool haveAdditionalScrollOptions = false, List<List<string>> additionalAvailableOptions = null) : base(textureSet.positions[0], textureSet.texture)
        {
            this.question = question;
            this.textureSet = textureSet;

            this.yesAndNo = yesAndNo;
            this.haveCancelButton = haveCancelButton;
            this.haveScrollOptions = haveScrollOptions;
            this.availableOptions = availableOptions;
            this.haveAdditionalScrollOptions = haveAdditionalScrollOptions;
            this.additionalAvailableOptions = additionalAvailableOptions;
            finished = false;
            cancelled = false;

            transform.localPosition = textureSet.positions[(int)DisplayOptionsParts.Body];
            questionTextObj = new TextObject(this, question, this.textureSet.fonts[(int)DisplayOptionsParts.Question], textureSet.positions[(int)DisplayOptionsParts.Question], TextRelativePosition.Center);
            if (yesAndNo)
            {
                yesButton = new Button(textureSet.positions[(int)DisplayOptionsParts.YesButton], textureSet.yesText, textureSet.fonts[(int)DisplayOptionsParts.YesButton], textureSet.yesButtonTextures, textureSet.positions[(int)DisplayOptionsParts.YesButtonSize], this);
                noButton = new Button(textureSet.positions[(int)DisplayOptionsParts.NoButton], textureSet.noText, textureSet.fonts[(int)DisplayOptionsParts.NoButton], textureSet.noButtonTextures, textureSet.positions[(int)DisplayOptionsParts.NoButtonSize], this);
                cancelButton = new Button(textureSet.positions[(int)DisplayOptionsParts.CancelButton], textureSet.cancelText, textureSet.fonts[(int)DisplayOptionsParts.CancelButton], textureSet.cancelButtonTextures, textureSet.positions[(int)DisplayOptionsParts.CancelButtonSize], this);
                yesButton.transform.layerDepth = -0.01f;
                noButton.transform.layerDepth = -0.01f;
                cancelButton.transform.layerDepth = -0.01f;
                yesButton.OnButtonActivationEvent += new Button.ButtonDelegate(Yes);
                noButton.OnButtonActivationEvent += new Button.ButtonDelegate(No);
                cancelButton.OnButtonActivationEvent += new Button.ButtonDelegate(Cancel);
            }

        }

        public virtual void Yes(Button button, Button.ButtonEventArgs e)
        {
            DestroyAndChildren();
            finished = true;
            yesOptionEvent?.Invoke(this, null);
        }

        public virtual void No(Button button, Button.ButtonEventArgs e)
        {
            DestroyAndChildren();
            finished = true;
            noOptionEvent?.Invoke(this, null);
        }

        public virtual void Cancel(Button button, Button.ButtonEventArgs e)
        {
            DestroyAndChildren();
            cancelled = true;
            finished = true;
            cancelOptionEvent?.Invoke(this, null);
        }

        public class DisplayOptionsEventArgs : EventArgs
        {

        }

        public delegate void DisplayOptionsDelegate(DisplayOptions dO, DisplayOptionsEventArgs e);

        public event DisplayOptionsDelegate yesOptionEvent;
        public event DisplayOptionsDelegate noOptionEvent;
        public event DisplayOptionsDelegate cancelOptionEvent;
    }
}