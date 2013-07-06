using System;
using System.Collections.Generic;
using IcicleFramework.Actions;
using IcicleFramework.Pooling;
using Microsoft.Xna.Framework;

namespace IcicleFramework.GameServices.Factories
{
    public class ActionFactory : GameService, IActionFactory
    {
        protected Dictionary<Type, IGameAction> cachedActions;

        protected MultiTypePool<IGameAction> actionPools;  

        public ActionFactory()
        {
            cachedActions = new Dictionary<Type, IGameAction>(128);
            actionPools = new MultiTypePool<IGameAction>(256);
        }

        public override void Update(GameTime gameTime)
        {
            actionPools.CleanUp();

            base.Update(gameTime);
        }

        public bool PreloadAction<T>() where T : class, IGameAction
        {
            return PreloadAction(typeof(T));
        }

        public bool PreloadAction(Type type)
        {
            var loaded = true;

            if (!actionPools.HasPool(type))
            {
                actionPools.CreatePool(type);
            }

            return loaded;
        }

        public T GetAction<T>() where T : class, IGameAction
        {
            return GetAction(typeof(T)) as T;
        }

        public IGameAction GetAction(Type type)
        {
            var action = actionPools.New(type);
            action.Initialize();

            return action;
        }
    }
}
