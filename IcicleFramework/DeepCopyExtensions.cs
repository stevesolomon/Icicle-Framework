using System;

namespace IcicleFramework
{
    public static class DeepCopyExtensions
    {
        public static T DeepClone<T>(this T item) where T : IDeepCopyable<T>, new()
        {
            var newThing = new T(); 
            item.CopyInto(newThing); 
            return newThing;
        }

        /// <summary>
        /// Casts a specific IDeepCopyable object from TSource to TConvert.
        /// </summary>
        /// <typeparam name="TSource">The source type of the IDeepCopyable object.</typeparam>
        /// <typeparam name="TConvert">The desired type of the IDeepCopyable object.</typeparam>
        /// <returns>A converted IDeepCopyable object.</returns>
        public static TConvert CastToType<TSource, TConvert>(this TSource item) 
            where TSource : class, IDeepCopyable<TSource>
            where TConvert : class, IDeepCopyable<TSource>
        {
            var converted = item as TConvert;

            if (converted == null)
            {
                throw new ArgumentException(string.Format("item was not of type {0}"), typeof(TConvert).FullName);
            }

            return converted;
        }
    }
}
