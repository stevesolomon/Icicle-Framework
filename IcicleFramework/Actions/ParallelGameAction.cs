using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Actions
{
    public class ParallelGameAction : CompositeGameAction
    {
        protected List<IGameAction> toRemove;

        public ParallelGameAction()
        {
            toRemove = new List<IGameAction>();
        }

        public override void Update(GameTime gameTime)
        {
            if (Paused)
                return;

            toRemove.Clear();

            foreach (var action in actions)
            {
                //Remove any actions that is marked as destroyed
                //and do not update them!
                if (action.Destroyed)
                {
                    toRemove.Add(action);
                }
                else
                {
                    action.Update(gameTime);
                }

                if (action.Finished)
                {
                    toRemove.Add(action);
                }
            }

            foreach (var action in toRemove)
            {
                actions.Remove(action);
            }
        }
    }
}
