using Microsoft.Xna.Framework;

namespace Monochrome.Entities
{
    public interface IMovementComponent : IBaseComponent
    {
        void Update(GameTime gameTime);
    }
}
