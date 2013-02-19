using System;
using IcicleFramework.Entities;
using IcicleFramework.Pooling;

namespace IcicleFramework.Components.Behaviors
{
    public interface IBehavior : IXmlBuildable, IUpdateable, IPoolable<IBehavior>, IInitializable, IPauseable, INameable, IDeepCopyable<IBehavior>, IDestroyable<IBehavior>, IDisposable
    {
        IBehaviorComponent Parent { get; set; }

        IGameObject ParentGameObject { get; }

        bool Active { get; set; }

        Type ConcreteType { get; set; }
    }
}
