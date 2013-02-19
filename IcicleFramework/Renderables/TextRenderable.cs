using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables
{
    public class TextRenderable : BaseRenderable, ITextRenderable
    {
        protected string spriteFontName;

        public string Text { get; set; }

        public SpriteFont Font { get; set; }

        public override int Width { get { return (int) Font.MeasureString(Text).X;  } }

        public override int Height { get { return (int)Font.MeasureString(Text).Y; } }

        public TextRenderable() { }

        protected TextRenderable(TextRenderable old)
            : base(old)
        {
            Text = old.Text;
            Font = old.Font;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Position + Offset, ColorChannel, Rotation, Origin, Scale, Effects, Layer);
        }

        public override void Load(ContentManager content)
        {
            Font = content.Load<SpriteFont>(spriteFontName);
        }

        public override void Dispose() { }

        public override IRenderable DeepClone()
        {
            return new TextRenderable(this);
        }

        public override void Deserialize(XElement element)
        {
            this.Text = "";
            if (element.Element("text") != null)
            {
                this.Text = element.Element("text").Value;
            }

            spriteFontName = null;
            if (element.Element("font") != null)
            {
                this.spriteFontName = element.Element("font").Value;
            }

            base.Deserialize(element);
        }

        
    }
}
