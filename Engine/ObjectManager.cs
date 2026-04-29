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
    public enum LayerDepths { Ground, ContextMenu }

    public class ObjectManager : GameObject
    {
        public List<GameObject> gameObjects;

        public ObjectManager(Vector2 position, Texture2D texture = null, GameObject parent = null) : base(position, texture, parent)
        {
            gameObjects = new List<GameObject>();
        }

        public GameObject AddObject(GameObject gameObject, bool alsoParent = false)
        {
            gameObjects.Add(gameObject);
            EngManager.gameObjects.Remove(gameObject);

            if (alsoParent)
            {
                gameObject.SetParent(this);
            }

            return gameObject;
        }

        public override void Update()
        {
            base.Update();
            UpdateGameObjects();
            DestroyGameObjects();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            DrawGameObjects();
        }

        protected virtual void UpdateGameObjects()
        {
            List<GameObject> gameObjectsThisFrame = new List<GameObject>(gameObjects);
            foreach (GameObject g in gameObjectsThisFrame)
            {
                g.Update();
            }
        }

        protected virtual void DestroyGameObjects()
        {
            List<GameObject> oldGameObjectList = new List<GameObject>(gameObjects);
            foreach (GameObject g in oldGameObjectList)
            {
                if (g.destroy == true)
                {
                    gameObjects.Remove(g);
                }
            }
        }

        protected virtual void DrawGameObjects()
        {
            foreach (GameObject g in gameObjects)
            {
                g.Draw(EngManager.spriteBatch);
            }
        }
    }
}