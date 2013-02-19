using Microsoft.Xna.Framework;

namespace Monochrome
{
    public interface IBaseComponent
    {
        void Update(GameTime gameTime);

        IBaseComponent DeepClone();
    }
}
