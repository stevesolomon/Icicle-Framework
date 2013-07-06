using System;
using IcicleFramework.Entities;
using IcicleFramework.Pooling;

namespace IcicleFramework.Components
{
    public interface IBaseComponent : IXmlBuildable, IUpdateable, IInitializable, IPauseable, IPoolable<IBaseComponent>, IDeepCopyable<IBaseComponent>
    {
        /// <summary>
        /// Gets whether or not this IBaseComponent is Active.
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// Gets or sets the Parent IGameObject that owns this IBaseComponent.
        /// </summary>
        IGameObject Parent { get; set; }

        /// <summary>
        /// Gets or sets the base interface type of this IBaseComponent.
        /// </summary>
        Type InterfaceType { get; set; }

        /// <summary>
        /// Gets or sets the concrete type of this IBaseComponent.
        /// </summary>
        Type ConcreteType { get; set; }
    }
}
