using IcicleFramework.Entities;

namespace IcicleFramework.Pooling
{
    public interface IPoolable<T> : IDestroyable<T>
    {
        void Reallocate();
    }
}
