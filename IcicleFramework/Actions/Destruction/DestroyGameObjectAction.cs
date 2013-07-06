using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions.Destruction
{
    public class DestroyGameObjectAction : GameAction
    {
        public override void Update(GameTime gameTime)
        {
            Target.Destroy();
            Finished = true;

            base.Update(gameTime);
        }
    }
}
