using IcicleFramework.Entities;

namespace IcicleFramework.Components
{
    /// <summary>
    /// A simple interface for any object that acts as a component for an <see cref="IBaseComponent"/>.
    /// </summary>
    public interface ISubComponent : IInitializable, IPauseable, INameable
    {
        IGameObject ParentGameObject { get; }
    }
}
