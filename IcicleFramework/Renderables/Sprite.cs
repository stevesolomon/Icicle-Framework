using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables
{
    /// <summary>
    /// The abstract Sprite class that all concrete Sprite classes will inherit from.
    /// </summary>
    public class Sprite : TexturedRenderable
    {
        #region Constructors

        public Sprite()
        {
            
        }

        public Sprite(string textureAsset)
            : this()
        {
            this.AssetName = textureAsset;
        }

        public Sprite(Texture2D texture)
            : this()
        {
            this.texture = texture;
            Source = new Rectangle(0, 0, texture.Width, texture.Height);

            this.AssetName = texture.Name;
        }

        protected Sprite(Sprite old)
            :base(old)
        { }

        #endregion


        #region IRenderable Methods

        /// <summary>
        /// Draws the Sprite to the screen using the provided SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use when drawing to the screen.</param>
       // public override void Draw(SpriteBatch spriteBatch)
       // {
      //      if (parent != null)
      //          spriteBatch.Draw(texture, (Position + Offset) + Origin, Parent, ColorChannel, Rotation, Origin, Scale, SpriteEffects.None, 1.0f);
      //  }
        
        #endregion


        #region IClonable Methods

        public override IRenderable DeepClone()
        {
            return new Sprite(this);
        }

        #endregion

    }
}
