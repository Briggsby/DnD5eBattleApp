using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BugsbyEngine
{
    public enum TextRelativePosition { Null, Center, Left }
    public class TextObject
    {
        public Transform transform;

        public string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public SpriteFont font;

        public TextRelativePosition textRelativePosition;

        public TextObject(GameObject gameObject, string text, SpriteFont font, Vector2 position, TextRelativePosition textRelativePosition = TextRelativePosition.Left)
        {
            this.font = font;
            this.textRelativePosition = textRelativePosition;
            gameObject.textObjects.Add(this);

            if (font == null)
            {
                this.font = EngManager.defaultFont;
            }

            this.text = text;

            transform = new Transform(gameObject.transform, null, position);
            transform.layerDepth -= 0.001f;
        }

        public virtual void Update()
        {

        }

        public Vector2 TextPosition(TextRelativePosition textRelativePosition = TextRelativePosition.Null)
        {
            if (textRelativePosition == TextRelativePosition.Null)
            {
                textRelativePosition = this.textRelativePosition;
            }
            switch (textRelativePosition)
            {
                case (TextRelativePosition.Left):
                    return transform.GlobalPosition;
                case (TextRelativePosition.Center):
                    return transform.GlobalPosition - font.MeasureString(text) / 2;
            }
            return transform.GlobalPosition;
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, TextPosition(), transform.color, transform.rotation, transform.origin, transform.scale, SpriteEffects.None, transform.LayerDepth);
        }
    }
}