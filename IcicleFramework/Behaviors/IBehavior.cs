using System;
using IcicleFramework.Components.Behaviors;
using IcicleFramework.Entities;
using IcicleFramework.Pooling;

namespace IcicleFramework.Behaviors
{
    public interface IBehavior : IXmlBuildable, IUpdateable, IPoolable<IBehavior>, IInitializable, IPauseable, INameable, IDeepCopyable<IBehavior>
    {
        IBehaviorComponent Parent { get; set; }

        IGameObject ParentGameObject { get; }

        bool Active { get; set; }

        Type ConcreteType { get; set; }
    }
}
