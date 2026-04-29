using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugsbyEngine
{
    public class GameObject
    {
        public bool active;

        public Transform transform;
        public Hitbox hitbox;
        public List<TextObject> textObjects;

        public bool destroy;


        public GameObject(Vector2 position, Texture2D texture = null, GameObject parent = null)
        {
            DefaultConstructor();
            if (parent == null)
            {
                transform = new Transform(texture, position);
            }
            else
            {
                SetParent(parent);
                transform = new Transform(parent.transform, texture, position);
            }
        }

        public GameObject(Vector2 position, HitboxShape shape, Vector3 dimensions, Texture2D texture, GameObject parent = null)
        {
            DefaultConstructor();
            if (parent != null)
            {
                SetParent(parent);
                transform = new Transform(parent.transform, texture, position);
            }
            else
            {
                transform = new Transform(texture, position);
            }

            MakeHitbox(new Vector2(0f,0f), shape, dimensions);
        }

        public GameObject(Vector2 position, Vector2 hitboxPosition, HitboxShape shape, Vector3 dimensions, Texture2D texture, GameObject parent = null)
        {
            DefaultConstructor();
            if (parent != null)
            {
                SetParent(parent);
                transform = new Transform(parent.transform, texture, position);
            }
            else
            {
                transform = new Transform(texture, position);
            }

            MakeHitbox(hitboxPosition, shape, dimensions);
        }

        public void DefaultConstructor()
        {
            active = true;
            destroy = false;
            EngManager.gameObjects.Add(this);
            transform = new Transform();
            hitbox = null;
            parent = null;
            children = new List<GameObject>();
            textObjects = new List<TextObject>();
        }

        #region Parents

        public GameObject parent;
        public List<GameObject> children;

        public void SetParent(GameObject parent, bool alsoTransform = false, bool alsoPosition = false, float layerDepth = -0.001f)
        {
            if (alsoTransform)
            {
                transform.SetParent(parent.transform, alsoPosition, layerDepth);
            }

            if (parent!= null)
            {
                RemoveParent();
            }

            this.parent = parent;
            parent.children.Add(this);
        }
        public void RemoveParent(bool alsoTransform = false)
        {
            if (alsoTransform)
            {
                transform.RemoveParent();
            }

            if (parent != null)
            {
                parent.children.Remove(this);
                parent = null;
            }
        }

        #endregion

        public Hitbox MakeHitbox()
        {
            Transform hitboxTransform = new Transform(this.transform, this.transform.Texture, new Vector2(0,0));
            return this.hitbox = new RectangleHitbox(hitboxTransform, new Vector3(transform.Size, 0), this);
        }

        public Hitbox MakeHitbox(Vector2 position, HitboxShape shape, Vector3 dimensions)
        {
            Transform hitboxTransform = new Transform(transform, null, position);

            if (shape == HitboxShape.Rectangle)
            {
            return hitbox = new RectangleHitbox(hitboxTransform, dimensions, this);
            }
            else
            {
                return null;
            }
        }

        public virtual void Update()
        {
            if (!active)
            {
                return;
            }

            if (transform != null)
            {
                transform.Update();
            }
            if (hitbox != null)
            {
                hitbox.Update();
            }
            if (textObjects != null)
            {
                foreach (TextObject tO in textObjects)
                {
                    tO.Update();
                }
            }

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!active)
            {
                return;
            }

            if (transform != null)
            {
                transform.Draw(spriteBatch);
            }
            if (textObjects != null)
            {
                foreach (TextObject tO in textObjects)
                {
                    tO.Draw(spriteBatch);
                }
            }
        }

        #region Transform Methods

        public void MoveTo(Vector2 position, bool moveChildren = false)
        {
            Vector2 oldPosition = transform.localPosition;
            transform.GlobalPosition = position;

            Vector2 translation = transform.localPosition - oldPosition;

            foreach (TextObject t in textObjects)
            {
                t.transform.localPosition -=-translation;
            }

            if (!moveChildren)
            {
                foreach (GameObject gO in children)
                {
                    gO.Translate(-translation);
                }
            }
        }

        public void Translate(Vector2 translation, bool moveChildren = false)
        {
            transform.localPosition += translation;
            if (!moveChildren)
            {
                foreach (GameObject gO in children)
                {
                    gO.Translate(-translation);
                }
            }
        }

        #endregion

        #region Hitbox functions

        public virtual void OnClick(){}
        public virtual void WhileClicked() { }
        public virtual void OnRightClick()
        {

        }
        public virtual void OnMouseOver()
        {

        }
        public virtual void OnMouseOff()
        {

        }
        public virtual void WhileMouseOver()
        {

        }
        public virtual void WhileMouseNotOver()
        {

        }

        #endregion

        #region Destruction

        public void Destroy()
        {
            active = false;
            OnSetDestroy();
            if (hitbox != null)
            {
                hitbox.active = false;
                hitbox.OnSetDestroy();
            }
            destroy = true;
        }

        public void DestroyAndChildren()
        {
            active = false;
            OnSetDestroy();
            if (hitbox != null)
            {
                hitbox.active = false;
                hitbox.OnSetDestroy();
            }
            foreach (GameObject g in children)
            {
                g.DestroyAndChildren();
            }
            destroy = true;
        }

        public virtual void OnSetDestroy()
        {

        }

        #endregion

    }
}