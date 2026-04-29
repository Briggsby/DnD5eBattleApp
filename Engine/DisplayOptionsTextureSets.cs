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
    enum DisplayOptionsParts { Body, Question, YesButton, NoButton, YesButtonSize, NoButtonSize, OptionScrollMenu, OptionScrollMenuSize, CancelButton, CancelButtonSize }

    public struct DisplayOptionsTextureSet
    {
        public Texture2D texture;
        public List<Vector2> positions;
        public List<Texture2D> yesButtonTextures;
        public List<Texture2D> noButtonTextures;
        public List<Texture2D> cancelButtonTextures;
        public List<Texture2D> scrollMenuTextures;
        public List<SpriteFont> fonts;

        public List<Vector2> additionalScrollAreaPositions;

        public string yesText;
        public string noText;
        public string cancelText;

        public DisplayOptionsTextureSet(Texture2D texture = null,
        List<Vector2> positions = null, List<Texture2D> yesButtonTextures = null,
        List<Texture2D> noButtonTextures = null, List<Texture2D> cancelButtonTextures = null,
        List<Texture2D> scrollMenuTextures = null, List<SpriteFont> fonts = null,
        List<Vector2> additionalScrollAreaPositions = null,
        string yesText = "Yes", string noText = "No", string cancelText = "Cancel")
        {
            this.texture = texture;
            if (positions == null) { this.positions = new List<Vector2>(); }
            else { this.positions = positions; }
            if (yesButtonTextures == null) { this.yesButtonTextures = new List<Texture2D>(); }
            else { this.yesButtonTextures = yesButtonTextures; }
            if (noButtonTextures == null) { this.noButtonTextures = new List<Texture2D>(); }
            else { this.noButtonTextures = noButtonTextures; }
            if (cancelButtonTextures == null) { this.cancelButtonTextures = new List<Texture2D>(); }
            else { this.cancelButtonTextures = cancelButtonTextures; }
            if (scrollMenuTextures == null) { this.scrollMenuTextures = new List<Texture2D>(); }
            else { this.scrollMenuTextures = scrollMenuTextures; }
            if (fonts == null) { this.fonts = new List<SpriteFont>(); }
            else { this.fonts = fonts; }
            if (additionalScrollAreaPositions == null) { this.additionalScrollAreaPositions = new List<Vector2>(); }
            else { this.additionalScrollAreaPositions = additionalScrollAreaPositions; }
            this.yesText = yesText;
            this.noText = noText;
            this.cancelText = cancelText;
        }


        public DisplayOptionsTextureSet(DisplayOptionsTextureSet otherSet)
        {
            texture = otherSet.texture;
            positions = otherSet.positions;
            yesButtonTextures = otherSet.yesButtonTextures;
            noButtonTextures = otherSet.noButtonTextures;
            cancelButtonTextures = otherSet.cancelButtonTextures;
            scrollMenuTextures = otherSet.scrollMenuTextures;
            fonts = otherSet.fonts;
            additionalScrollAreaPositions = otherSet.additionalScrollAreaPositions;
            yesText = otherSet.yesText;
            noText = otherSet.noText;
            cancelText = otherSet.cancelText;
        }
    }
}