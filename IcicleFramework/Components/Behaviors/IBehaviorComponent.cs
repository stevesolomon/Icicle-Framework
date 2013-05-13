using System.Collections.Generic;
using IcicleFramework.Actions;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.EntityState;
using IcicleFramework.Entities;
using IcicleFramework.GameServices;
using IcicleFramework.GameServices.Factories;

namespace IcicleFramework.Components.Behaviors
{
    public interface IBehaviorComponent : IBaseComponent, IEntityStateModifier
    {
        IEnumerator<IBehavior> Behaviors { get; }

        IActionFactory ActionFactory { get; }

        IBehavior GetBehavior(string name);

        void AddBehavior(IBehavior behavior);

        void RemoveBehavior(IBehavior behavior);

        void FireAction(IGameAction action, IGameObject target, ActionCompletedCallback callback = null, float delay = 0f);
    }
}
