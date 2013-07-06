namespace IcicleFramework.Pooling
{
    interface IPoolNew<T> where T : class, IPoolable<T>
    {
        T New();

        void CleanUp();
    }
}
