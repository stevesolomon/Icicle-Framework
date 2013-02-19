using IcicleFramework.Components;

namespace IcicleFramework
{
    public interface IComposite : IUpdateable
    {
        void AddComponent(IBaseComponent component);
        void RemoveComponent(IBaseComponent remove);
        T RetrieveComponentOfType<T>();
    }
}
