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
    public class EngManager
    {
        public static Controls controls;
        public static Random random;
        public static List<GameObject> gameObjects;
        public static Camera mainCamera;

        public static List<Coroutine> coroutines;

        public static GameTime gameTime;
        public static SpriteBatch spriteBatch;
        public static GraphicsDeviceManager graphics;

        public static SpriteFont defaultFont;

        public static Dictionary<LayerDepths, float> layerDepths = new Dictionary<LayerDepths, float>()
        {
            {LayerDepths.Ground, 0.95f },
            {LayerDepths.ContextMenu, 0.05f }
        };

        public bool managerManageObjects;


        public float uiLayer = 1.0f;

        public EngManager(SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Vector2 windowSize, bool managerManageObjects = true)
        {
            controls = new Controls();
            random = new Random();
            gameObjects = new List<GameObject>();
            mainCamera = new Camera(windowSize);
            coroutines = new List<Coroutine>();

            this.managerManageObjects = managerManageObjects;

            EngManager.spriteBatch = spriteBatch;
            EngManager.graphics = graphics;
        }

        public void Update(GameTime gameTime)
        {
            EngManager.gameTime = gameTime;

            controls.Update();

            if (managerManageObjects) {
                UpdateGameObjects();
                DestroyGameObjects();

            }

            UpdateCoroutines();
        }

        public void BeginSpriteBatch()
        {
            //spriteBatch.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: mainCamera.GetMatrix() * Matrix.CreateScale(mainCamera.GetScreenScale()));
            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront);
        }

        public void Draw()
        {
            if (managerManageObjects)
            {
                DrawGameObjects();
            }
        }

        void UpdateGameObjects()
        {
            List<GameObject>gameObjectsThisFrame = new List<GameObject>(gameObjects);
            foreach (GameObject g in gameObjectsThisFrame)
            {
                g.Update();
            }
        }

        void DestroyGameObjects()
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

        void DrawGameObjects()
        {
            foreach (GameObject g in gameObjects)
            {
                g.Draw(spriteBatch);
            }
        }

        #region Coroutines


        private void UpdateCoroutines()
        {
            List<Coroutine> coroutinesToUpdate = new List<Coroutine>(coroutines);
            foreach (Coroutine c in coroutinesToUpdate)
            {
                c.Step();
            }
        }

        public static void StartCoroutine(IEnumerator coroutine, bool limitToOneInstance = false, string name = null)
        {
            if (limitToOneInstance)
            {
                foreach (Coroutine c in coroutines)
                {
                    if (c.Name == name)
                    {
                        return;
                    }
                }
            }
            coroutines.Add(new Coroutine(name, coroutine));
        }

        public static void StopCoroutine(IEnumerator coroutine, string name = null, bool all = false)
        {
            if (name != null)
            {
                foreach (Coroutine c in coroutines)
                {
                    if (c.Name == name)
                    {
                        coroutines.Remove(c);
                        if (!all)
                        {
                            return;
                        }
                    }
                }
            }

            else
            {
                foreach (Coroutine c in coroutines)
                {
                    if (c.generator.GetType() == coroutine.GetType())
                    {
                        coroutines.Remove(c);
                        if (!all)
                        {
                            return;
                        }
                    }
                }
            }
        }

        #endregion
    }
}