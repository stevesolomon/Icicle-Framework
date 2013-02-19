using IcicleFramework.Actions;
using Microsoft.Xna.Framework;

namespace ExampleGameSHMUP.Actions.Projectiles
{
    public class BaseDestroyProjectileAction : GameAction
    {
        public override void Update(GameTime gameTime)
        {
            if (Parent != null)
            {
                Parent.Destroyed = true;
            }

            Finished = true;

            base.Update(gameTime);
        }
    }
}
