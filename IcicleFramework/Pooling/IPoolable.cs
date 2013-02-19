namespace IcicleFramework.Pooling
{
    public interface IPoolable<T>
    {
        bool Unallocated { get; }
    }
}
