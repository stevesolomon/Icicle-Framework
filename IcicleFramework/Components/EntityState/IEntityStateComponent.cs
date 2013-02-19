using IcicleFramework.Entities;

namespace IcicleFramework.Components.EntityState
{
    public delegate void OnEntityStateChangedHandler(IGameObject source, IEntityState newState);

    public interface IEntityStateComponent : IBaseComponent
    {
        IEntityState State { get; }

        bool ChangeState(IEntityState newState);

        event OnEntityStateChangedHandler OnEntityStateChanged;
    }
}
