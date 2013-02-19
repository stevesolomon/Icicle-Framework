using System.Globalization;
using System.Xml.Linq;
using IcicleFramework.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables
{
    public abstract class BaseRenderable : IRenderable
    {
        protected IBaseComponent parent;

        protected float layer;

        protected Vector2 nonOffsetPosition;

        public bool Visible { get; set; }

        public bool VisibleOnScreen { get; protected set; }

        public float Scale { get; set; }

        public Color ColorChannel { get; set; }

        public Vector2 Offset { get; set; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        public Vector2 Position
        {
            get { return nonOffsetPosition + Offset; }
            set { nonOffsetPosition = value - Offset; }
        }

        public IBaseComponent Parent
        {
            get { return parent; }
            set
            {
                if (value != null)
                    parent = value;
            }
        }

        public virtual float Layer
        {
            get { return layer; }
            set
            {
                if (value < 0 || value > 1)
                    layer = 0.5f;
                else
                    layer = value;
            }
        }

        /// <summary>
        /// Gets or sets the SpriteEffects for this Renderable.
        /// </summary>
        public SpriteEffects Effects { get; set; }

        /// <summary>
        /// Gets or sets the origin point used for translating the position to draw this Renderable.
        /// </summary>
        public Vector2 Origin { get; set; }

        /// <summary>
        /// Gets or sets the current rotational value for this Renderable.
        /// </summary>
        public float Rotation { get; set; }

        protected BaseRenderable()
        {
            this.Origin = new Vector2(0f, 0f);
            this.Scale = 1.0f;
            this.layer = 1f;
            this.ColorChannel = Color.White;
            this.Offset = Vector2.Zero;
            this.Effects = SpriteEffects.None;
            this.Rotation = 0.0f;
        }

        protected BaseRenderable(BaseRenderable old)
        {
            this.Origin = old.Origin;
            this.Scale = old.Scale;
            this.Layer = old.Layer;
            this.ColorChannel = old.ColorChannel;
            this.Offset = old.Offset;
            this.Effects = old.Effects;
            this.Rotation = old.Rotation;
        }

        public virtual void Initialize() {}

        public abstract void Draw(SpriteBatch spriteBatch);

        public abstract void Load(ContentManager content);

        public virtual void Update(GameTime gameTime)
        {
            nonOffsetPosition = Parent.Parent.Position;
            
            //If our parent IGameObject has rotated, then add its last rotation amount to our current rotation angle.
            if (Parent.Parent.HasRotated)
            {
                Rotation += Parent.Parent.LastRotationAmount;
            }
        }

        public abstract IRenderable DeepClone();

        public virtual void Deserialize(XElement element)
        {
            float scale = 1.0f;
            if (element.Element("scale") != null)
            {
                float.TryParse(element.Element("scale").Value, NumberStyles.Float, CultureInfo.InvariantCulture,
                                out scale);
            }
            this.Scale = scale;

            float layer = 1.0f;
            if (element.Element("layer") != null)
            {
                float.TryParse(element.Element("layer").Value, NumberStyles.Float, CultureInfo.InvariantCulture,
                               out layer);
            }
            this.Layer = layer;

            Color color = Color.White;
            if (element.Element("color") != null)
            {
                color = color.ConvertColorFromString(element.Element("color").Value);
            }
            this.ColorChannel = color;

            Vector2 offset = Vector2.Zero;
            if (element.Element("offset") != null)
            {
                offset = offset.DeserializeOffset(element.Element("offset"));
            }
            this.Offset = offset;

            bool yMirror = false, xMirror = false;
            var mirrorElem = element.Element("mirror");
            if (mirrorElem != null)
            {
                var attrib = mirrorElem.Attribute("y");
                if (attrib != null)
                {
                    bool.TryParse(attrib.Value, out yMirror);
                }

                attrib = mirrorElem.Attribute("x");
                if (attrib != null)
                {
                    bool.TryParse(attrib.Value, out xMirror);
                }
            }

            Effects = SpriteEffects.None;

            if (yMirror && xMirror)
            {
                Effects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
            }
            else if (yMirror)
            {
                Effects = SpriteEffects.FlipVertically;
            }
            else if (xMirror)
            {
                Effects = SpriteEffects.FlipHorizontally;
            }
        }

        public abstract void Dispose();
    }
}
