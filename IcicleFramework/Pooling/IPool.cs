using System;

namespace IcicleFramework.Pooling
{
    public interface IPool<T> where T : class, IPoolable<T>
    {
        /// <summary>
        /// Gets or sets a delegate used for initializing objects before returning them from the New method.
        /// </summary>
        Action<T> Initialize { get; set; }

        /// <summary>
        /// Gets or sets a delegate that is run when an object is moved from being valid to invalid
        /// in the CleanUp method.
        /// </summary>
        Action<T> Deinitialize { get; set; }

        /// <summary>
        /// Gets the number of valid objects in the pool.
        /// </summary>
        int ValidCount { get; }

        /// <summary>
        /// Gets the number of invalid objects in the pool.
        /// </summary>
        int InvalidCount { get; }

        /// <summary>
        /// Returns a valid object at the given index. The index must fall in the range of [0, ValidCount].
        /// </summary>
        /// <param name="index">The index of the valid object to get</param>
        /// <returns>A valid object found at the index</returns>
        T this[int index] { get; }

        /// <summary>
        /// Cleans up the pool by checking each valid object to ensure it is still actually valid.
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Returns a new object from the Pool.
        /// </summary>
        /// <returns>The next object in the pool if available, null if all instances are valid.</returns>
        T New();
    }
}
