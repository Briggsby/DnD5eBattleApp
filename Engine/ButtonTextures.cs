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
    public enum ButtonTexture { BaseTexture = 0, HoveredTexture = 1, PressedTexture = 2, PressedHoveredTexture = 3, InactiveTexture = 4 };

    public class ButtonTextures
    {
        public Texture2D baseTexture;
        public Texture2D hoveredTexture;
        public Texture2D pressedTexture;
        public Texture2D pressedHoveredTexture;
        public Texture2D inactiveTexture;

        public ButtonTextures()
        {
            this.baseTexture =              null;
            this.hoveredTexture =           null;
            this.pressedTexture =           null;
            this.pressedHoveredTexture =    null;
            this.inactiveTexture =          null;
        }

        public ButtonTextures(Texture2D baseTexture, Texture2D hoveredTexture, Texture2D pressedTexture, Texture2D pressedHoveredTexture, Texture2D inactiveTexture)
        {
            this.baseTexture = baseTexture;
            this.hoveredTexture = hoveredTexture;
            this.pressedTexture = pressedTexture;
            this.pressedHoveredTexture = pressedHoveredTexture;
            this.inactiveTexture = inactiveTexture;
        }

        public List<Texture2D> ToList()
        {
            return new List<Texture2D>() { baseTexture, hoveredTexture, pressedTexture, pressedHoveredTexture, inactiveTexture };
        }

        public static ButtonTextures FromList(List<Texture2D> textures)
        {
            return new ButtonTextures(textures[0], textures[1], textures[2], textures[3], textures[4]);
        }
    }
}
