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
    public class Camera
    {

        public Vector2 size;
        public Transform transform;

        public Camera(Vector2 size)
        {
            this.size = size;
            transform = new Transform();
        }

        public Vector3 GetScreenScale()
        {
            return new Vector3((float)EngManager.graphics.GraphicsDevice.Viewport.Width / (float)size.X, (float)EngManager.graphics.GraphicsDevice.Viewport.Height / (float)size.Y, 1);
        }

        public Matrix GetMatrix()
        {
            Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(transform.Position().X, transform.Position().Y, 0));
            Matrix rotationMatrix = Matrix.CreateRotationZ(transform.rotation);
            Matrix scaleMatrix = Matrix.CreateScale(new Vector3(transform.scale.X, transform.scale.Y, 1));
            Matrix originMatrix = Matrix.CreateTranslation(new Vector3(transform.Position().X, transform.Position().Y, 0));

            return translationMatrix * rotationMatrix * scaleMatrix * originMatrix;
        }
    }
}