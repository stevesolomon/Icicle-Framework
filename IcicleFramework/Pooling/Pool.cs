using System;
using System.Diagnostics;
using System.Reflection;

namespace IcicleFramework.Pooling
{
    /// <summary>
    /// Creates a new Pool for storing T objects.
    /// Generics are being manhandled a bit here, in order to support MultiTypePooling. As a result, the 
    /// Pool constructor still requires a hard Type object passed in, despite the class supporting generics.
    /// This covers the case where we create a Pool<ISomething> and only know at run-time that it's storing
    /// Something : ISomething. In this way, we can still return objects as the high level interface. Calling
    /// classes that know the type at design time can perform the cast, everything else will just have to work
    /// with the high-level interface, as the alternative is lots of reflection which is clearly not ideal here. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> : IPool<T> where T : class, IPoolable<T>, IDisposable
    {
        // the amount to enlarge the items array if New is called and there are no free items
        private const int ResizeAmount = 20;

        // the actual items of the pool
        private T[] items;

        // a constructor the default allocate method can use to create instances
        private readonly ConstructorInfo constructor;

        /// <summary>
        /// A delegate that returns a new object instance for the Pool.
        /// </summary>
        /// <returns>A new object instance.</returns>
        public delegate T Allocate();

        /// <summary>
        /// Gets or sets a delegate used for initializing objects before returning them from the New method.
        /// </summary>
        public Action<T> Initialize { get; set; }

        /// <summary>
        /// Gets or sets a delegate that is run when an object is moved from being valid to invalid
        /// in the CleanUp method.
        /// </summary>
        public Action<T> Deinitialize { get; set; }

        /// <summary>
        /// Gets the number of valid objects in the pool.
        /// </summary>
        public int ValidCount { get { return items.Length - InvalidCount; } }

        /// <summary>
        /// Gets the number of invalid objects in the pool.
        /// </summary>
        public int InvalidCount { get; private set; }

        protected Type ConcreteType { get; set; }

        public T this[int index]
        {
            get
            {
                index += InvalidCount;

                if (index < InvalidCount || index >= items.Length)
                    throw new IndexOutOfRangeException("The index must be less than or equal to ValidCount");

                return items[index];
            }
        }

        public Pool(Type type, int initialSize = 128)
        {
            //For obvious reasons we must have an initial size greater than 0!
            if (initialSize < 0)
                throw new ArgumentException("initialSize must be non-negative");

            if (initialSize == 0)
                initialSize = 10;

            items = new T[initialSize];

            InvalidCount = items.Length;

            ConcreteType = type;

            //Find a valid, parameterless constructor to use for the passed in Type.
            constructor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new Type[] { },
                null);

            if (constructor == null)
                throw new InvalidOperationException(typeof(T) + " does not have a parameterless constructor.");
        }

        public void CleanUp()
        {
            for (int i = InvalidCount; i < items.Length; i++)
            {
                T obj = items[i];

                //If the object hasn't been destroyed then keep going.
                if (!obj.Destroyed)
                    continue;

                //If the object has been destroyed and we're not at the start of the invalid objects,
                //we have to move the object to the invalid section of the array.
                if (i != InvalidCount)
                {
                    items[i] = items[InvalidCount];
                    items[InvalidCount] = obj;
                }

                // clean the object if desired
                if (Deinitialize != null)
                    Deinitialize(obj);

                obj.Dispose();

                InvalidCount++;
            }
        }

        public T New()
        {
            // if we're out of invalid instances, resize to fit some more
            if (InvalidCount == 0)
            {
#if WINDOWS
                Trace.WriteLine("Resizing single type pool storing " + ConcreteType.FullName + ". Old size: " + items.Length + ". New size: " + (items.Length + ResizeAmount));
#endif
                // create a new array with some more slots and copy over the existing items
                T[] newItems = new T[items.Length + ResizeAmount];
                for (int i = items.Length - 1; i >= 0; i--)
                    newItems[i + ResizeAmount] = items[i];
                items = newItems;

                // move the invalid count based on our resize amount
                InvalidCount += ResizeAmount;
            }

            // decrement the counter
            InvalidCount--;

            // get the next item in the list
            T obj = items[InvalidCount];

            // if the item is null, we need to allocate a new instance
            if (obj == null)
            {
                obj = ConstructorAllocate();

                if (obj == null)
                    throw new InvalidOperationException("The pool's allocate method returned a null object reference.");

                items[InvalidCount] = obj;
            }

            // initialize the object if a delegate was provided
            if (Initialize != null)
                Initialize(obj);

            return obj;
        }

        private T ConstructorAllocate()
        {
            return constructor.Invoke(null) as T;
        }
    }
}
