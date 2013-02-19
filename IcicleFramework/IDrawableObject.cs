using Microsoft.Xna.Framework.Graphics;

namespace IcicleFramework
{
    public interface IDrawableObject : ILoadable
    {
        bool Visible { get; set; }

        /// <summary>
        /// Gets whether or not this <see cref="IDrawableObject"/> is visible on screen.
        /// </summary>
        bool VisibleOnScreen { get; }

        void Draw(SpriteBatch spriteBatch);
    }
}
