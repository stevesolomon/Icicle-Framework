using System;
using System.Collections.Generic;
using System.Reflection;

namespace IcicleFramework.Pooling
{
    public class MultiTypePool<T> where T : class, IPoolable<T>
    {
        public int InitialSize { get; set; }

        protected Dictionary<Type, PoolNew<T>> typePools;

        public MultiTypePool(int initialSize = 128)
        {
            typePools = new Dictionary<Type, PoolNew<T>>();

            InitialSize = initialSize;
        }

        public void CreatePool(Type type)
        {
            if (!typePools.ContainsKey(type))
            {
                var pool = new PoolNew<T>(type, InitialSize);

                typePools.Add(type, pool);
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
