using System;
using System.Collections.Generic;
using System.Reflection;

namespace IcicleFramework.Pooling
{
    public class PoolNew<T> : IPoolNew<T> where T : class, IPoolable<T>
    {
        private readonly Queue<T> freeItems;

        private readonly List<T> allocatedItems;

        public int PoolSize { get; protected set; }

        protected ConstructorInfo Constructor { get; set; }
        
        public PoolNew(Type type, int initialCount = 128)
        {
            PoolSize = initialCount;

            Constructor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { },
                null
            );

            freeItems = new Queue<T>(128);
            allocatedItems = new List<T>(128);

            AllocateNewItems(PoolSize);
        }

        public T New()
        {
            var newItem = GetNewItem();

            newItem.OnDestroyed += OnItemDestroyed;

            return newItem;
        }

        private void OnItemDestroyed(T sender)
        {
            sender.OnDestroyed -= OnItemDestroyed;

            allocatedItems.Remove(sender);
            freeItems.Enqueue(sender);
        }

        /// <summary>
        /// Scans through all allocated items and determines if they can be cleaned up or not.
        /// This is a costly operation.
        /// </summary>
        public void CleanUp()
        {
            for (var i = allocatedItems.Count - 1; i >= 0; i--)
            {
                if (allocatedItems[i].Destroyed)
                {
                    var item = allocatedItems[i];
                    allocatedItems.RemoveAt(i);

                    freeItems.Enqueue(item);
                }
            }
        }

        protected T GetNewItem()
        {
            if (freeItems.Count == 0)
                AllocateNewItems(PoolSize * 2);

            var item = freeItems.Dequeue();
            item.Reallocate();

            allocatedItems.Add(item);

            return item;
        }

        protected void AllocateNewItems(int count)
        {
            for (var i = 0; i < count; i++)
            {
                freeItems.Enqueue(Constructor.Invoke(null) as T);
            }

            PoolSize = count;
        }
    }
}
