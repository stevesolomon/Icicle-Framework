namespace IcicleFramework.Components
{
    /// <summary>
    /// An interface used for any components that are scalable.
    /// </summary>
    public interface IScalable
    {
        /// <summary>
        /// Applies the given scaling value to this IScalable object.
        /// </summary>
        /// <param name="xScaling">The amount to scale along the x-axis.</param>
        /// <param name="yScaling">The amount to scale along the y-axis.</param>
        void ApplyScale(float xScaling, float yScaling);
    }
}
