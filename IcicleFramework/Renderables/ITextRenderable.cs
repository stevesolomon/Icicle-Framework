using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework.Renderables
{
    public interface ITextRenderable : IRenderable
    {
        /// <summary>
        /// Gets or sets the text to be displayed by this ITextRenderable.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the SpriteFont to use when displaying text.
        /// </summary>
        SpriteFont Font { get; set; }
    }
}
