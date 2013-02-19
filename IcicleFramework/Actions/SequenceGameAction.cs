using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions
{
    public class SequenceGameAction : CompositeGameAction
    {
        public override void Update(GameTime gameTime)
        {
            if (actions.Count > 0)
            {
                actions[0].Update(gameTime);

                if (actions[0].Finished)
                {
                    actions.Remove(actions[0]);
                }
            }

            Finished = actions.Count == 0;

            base.Update(gameTime);
        }
    }
}
