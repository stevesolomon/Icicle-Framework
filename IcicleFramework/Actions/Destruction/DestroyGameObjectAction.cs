using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions.Destruction
{
    public class DestroyGameObjectAction : GameAction
    {
        public override void Update(GameTime gameTime)
        {
            Target.Destroyed = true;
            Finished = true;

            base.Update(gameTime);
        }
    }
}
