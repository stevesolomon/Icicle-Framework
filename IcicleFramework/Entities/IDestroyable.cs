namespace IcicleFramework.Entities
{
    public delegate void DestroyedHandler<T>(T sender);

    public interface IDestroyable<T>
    {
        event DestroyedHandler<T> OnDestroyed;

        bool Destroyed { get; }

        void Destroy();
    }
}
