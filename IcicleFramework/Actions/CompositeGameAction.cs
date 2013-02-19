using System.Collections.Generic;
using IcicleFramework.Entities;

namespace IcicleFramework.Actions
{
    public abstract class CompositeGameAction : GameAction
    {
        protected List<IGameAction> actions;

        public override IGameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                if (value == target)
                    return;

                target = value;

                foreach (var action in actions)
                    action.Target = target;
            }
        }

        public override IGameObject Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                if (value == base.Parent)
                    return;

                base.Parent = value;

                foreach (var action in actions)
                    action.Parent = Parent;
            }
        }

        public CompositeGameAction()
        {
            actions = new List<IGameAction>();
        }

        public override void Initialize()
        {
            foreach (var action in actions)
                action.Initialize();

            base.Initialize();
        }

        public void AddAction(IGameAction action)
        {
            actions.Add(action);
        }

        public void RemoveAction(IGameAction action)
        {
            actions.Remove(action);
        }
    }
}
