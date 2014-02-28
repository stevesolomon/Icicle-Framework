using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace IcicleFramework.Renderables
{
    public abstract class TexturedRenderable : BaseRenderable, ITexturedRenderable
    {
        protected Texture2D texture;
        
        public string AssetName { get; set; }
        
        [JsonIgnore]
        public Texture2D Texture
        {
            get { return texture; }
            protected set { texture = value; }
        }

        /// <summary>
        /// Gets or sets the source area for the Sprite
        /// </summary>
        public Rectangle Source { get; set; }

        public override int Width { get { return Texture.Width; } }

        public override int Height { get { return Texture.Height; } }

        protected TexturedRenderable()
        {
            AssetName = null;
            texture = null;
        }

        protected TexturedRenderable(TexturedRenderable old)
            : base(old)
        {
            this.AssetName = old.AssetName;
            this.Source = old.Source;
            this.texture = old.texture;
        }
        
        public override void Load(ContentManager content)
        {
            if (AssetName != null)
            {
                texture = content.Load<Texture2D>(AssetName);
                Source = new Rectangle(0, 0, texture.Width, texture.Height);
            }
        }

        public override void Initialize()
        {
            Origin = new Vector2(this.Texture.Width / 2f, this.Texture.Height / 2f);
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position + Origin, Source, ColorChannel, Rotation, Origin, Scale, Effects, Layer);
        }

        public override void Dispose()
        {
            
        }
        
        public override void Deserialize(XElement element)
        {
            if (element.Element("asset") != null)
                this.AssetName = element.Element("asset").Value;

            Vector2 vect = Vector2.Zero;
            if (element.Element("size") != null)
            {
                vect = vect.DeserializeOffset(element.Element("size"));
            }
            this.Source = new Rectangle(0, 0, (int) vect.X, (int) vect.Y);
            
            base.Deserialize(element);
        }
    }
}
