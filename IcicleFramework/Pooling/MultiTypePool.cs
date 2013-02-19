using System;
using System.Collections.Generic;

namespace IcicleFramework.Pooling
{
    public class MultiTypePool<T> where T : class, IPoolable<T>, IDisposable
    {
        public int InitialSize { get; set; }

        protected Dictionary<Type, Pool<T>> typePools;

        protected Dictionary<Type, int> numRetrievedFromPool;

        protected int maxRetrievedFromPool = 128;

        public MultiTypePool(int initialSize = 128)
        {
            typePools = new Dictionary<Type, Pool<T>>();
            numRetrievedFromPool = new Dictionary<Type, int>();

            InitialSize = initialSize;
        }

        public Action<T> Initialize { get; set; }

        public Action<T> Deinitialize { get; set; }

        protected void CreatePool(Type type)
        {
            var pool = new Pool<T>(type, InitialSize);
            pool.Initialize = Initialize;
            pool.Deinitialize = Deinitialize;

            typePools.Add(type, pool);
            numRetrievedFromPool.Add(type, 0);
        }

        public void PreparePool(Type type)
        {
            if (!typePools.ContainsKey(type))
            {
                CreatePool(type);
            }
        }

        public bool HasPool(Type type)
        {
            return typePools.ContainsKey(type);
        }

        public void CleanUp()
        {
            foreach (var pool in typePools.Values)
            {
                pool.CleanUp();
            }
        }

        public T New(Type type)
        {
            //First check if we have this type available in our internal pools.
            //If not, then create a pool for the type.
            if (!typePools.ContainsKey(type))
            {
                CreatePool(type);
            }

            //Then pull an item from the pool and return it!
            var item = typePools[type].New();

            return item;
        }
    }
}
