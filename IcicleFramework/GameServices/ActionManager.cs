using System;
using System.Collections.Generic;
using IcicleFramework.Actions;
using Microsoft.Xna.Framework;

namespace IcicleFramework.GameServices
{
    public class ActionManager : GameService, IActionManager
    {
        protected ParallelGameAction executingActions;

        protected List<IGameAction> delayedActions;

        protected List<float> delayedActionsTime;

        protected List<IGameAction> addNextUpdate; 

        protected Dictionary<IGameAction, ActionCompletedCallback> callbacks; 
        
        public override void Update(GameTime gameTime)
        {
            //First add any new actions that were added in the previous Update.
            foreach (IGameAction action in addNextUpdate)
            {
                RegisterActionForExecution(action);
            }

            addNextUpdate.Clear();

            for (var i = delayedActionsTime.Count - 1; i >= 0; i--)
            {
                delayedActionsTime[i] -= (float) gameTime.ElapsedGameTime.TotalSeconds;

                //Remove the action and put it into the active executing actions if it's time.
                if (delayedActionsTime[i] <= 0.0f)
                {
                    executingActions.AddAction(delayedActions[i]);

                    delayedActions.RemoveAt(i);
                    delayedActionsTime.RemoveAt(i);
                }
            }

            executingActions.Update(gameTime);
        }

        public override void Initialize()
        {
            executingActions = new ParallelGameAction();
            delayedActions = new List<IGameAction>(128);
            delayedActionsTime = new List<float>(128);
            addNextUpdate = new List<IGameAction>(128);
            callbacks = new Dictionary<IGameAction, ActionCompletedCallback>();
        }

        public void RegisterAction(IGameAction action, ActionCompletedCallback callback = null)
        {
            addNextUpdate.Add(action);
            RegisterCallback(action, callback);
        }

        private void OnGameActionFinished(IGameAction source)
        {
            source.OnGameActionFinished -= OnGameActionFinished;

            if (callbacks.ContainsKey(source))
            {
                callbacks[source](source);
                callbacks.Remove(source);
            }
        }

        public void RegisterDelayedAction(IGameAction action, float delayInSeconds, ActionCompletedCallback callback = null)
        {
            delayedActions.Add(action);
            delayedActionsTime.Add(delayInSeconds);

            RegisterCallback(action, callback);
        }

        protected void RegisterActionForExecution(IGameAction action)
        {
            executingActions.AddAction(action);
        }

        protected void RegisterCallback(IGameAction action, ActionCompletedCallback callback)
        {
            if (callback != null && !callbacks.ContainsKey(action))
            {
                callbacks.Add(action, callback);
                action.OnGameActionFinished += OnGameActionFinished;
            }
        }
    }
}
